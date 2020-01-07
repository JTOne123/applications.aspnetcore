﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vostok.Commons.Time;
using Vostok.Context;
using Vostok.Hosting.AspNetCore.Configuration;
using Vostok.Hosting.AspNetCore.Models;
using Vostok.Logging.Abstractions;
using Vostok.Throttling;

namespace Vostok.Hosting.AspNetCore.Middlewares
{
    internal class ThrottlingMiddleware : IMiddleware
    {
        private const long LargeRequestBodySize = 4 * 1024;
        private static readonly TimeSpan LongThrottlingWaitTime = 500.Milliseconds();

        private readonly ThrottlingSettings settings;
        private readonly IThrottlingProvider provider;
        private readonly ILog log;

        public ThrottlingMiddleware(ThrottlingSettings settings, IThrottlingProvider provider, ILog log)
        {
            this.settings = settings;
            this.provider = provider;
            this.log = log;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var info = FlowingContext.Globals.Get<IRequestInfo>();
            var properties = BuildThrottlingProperties(context, info);

            using (var result = await provider.ThrottleAsync(properties, info.RemainingTimeout))
            {
                if (result.Status == ThrottlingStatus.Passed)
                {
                    if (result.WaitTime >= LongThrottlingWaitTime)
                        LogWaitTime(info, result);

                    await next(context);

                    return;
                }

                LogFailure(info, result);

                if (ShouldAbortConnection(context, result))
                {
                    LogAbortingConnection();
                    context.Abort();
                }
                else
                {
                    context.Response.StatusCode = settings.RejectionResponseCode;
                    context.Response.Headers.ContentLength = 0L;
                }
            }
        }

        // TODO(iloktionov): Chunked transfer encoding, web sockets?
        private static bool ShouldAbortConnection(HttpContext context, IThrottlingResult result)
            => result.Status == ThrottlingStatus.RejectedDueToDeadline || context.Request.ContentLength > LargeRequestBodySize;

        private static IReadOnlyDictionary<string, string> BuildThrottlingProperties(HttpContext context, IRequestInfo info)
        {
            // TODO(iloktionov): on/off switches for properties

            return new ThrottlingPropertiesBuilder()
                .AddConsumer(info.ClientApplicationIdentity)
                .AddPriority(info.Priority?.ToString())
                .AddMethod(context.Request.Method)
                .Build();
        }

        private void LogWaitTime(IRequestInfo info, IThrottlingResult result)
            => log.Warn(
                "Request from {ClientIdentity} spent {ThrottlingWaitTime} on throttling.",
                info.ClientApplicationIdentity,
                result.WaitTime);

        private void LogFailure(IRequestInfo info, IThrottlingResult result)
            => log.Error(
                "Dropping request from {ClientIdentity} due to throttling status {ThrottlingStatus}. Rejection reason = '{RejectionReason}'.",
                info.ClientApplicationIdentity,
                result.Status,
                result.RejectionReason);

        private void LogAbortingConnection()
            => log.Info("Aborting client connection..");
    }
}
