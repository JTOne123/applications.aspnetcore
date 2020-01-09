﻿using JetBrains.Annotations;
using Vostok.Throttling;
using Vostok.Throttling.Metrics;

namespace Vostok.Applications.AspNetCore.Configuration
{
    [PublicAPI]
    public class ThrottlingSettings
    {
        /// <summary>
        /// <para>HTTP code to respond with when a request fails to pass throttling.</para>
        /// <para>Note that in some circumstances (expired timeout, large request body) client connection may be aborted instead.</para>
        /// </summary>
        public int RejectionResponseCode { get; set; } = 429;

        /// <summary>
        /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Consumer"/> property to <see cref="IThrottlingProvider"/>.
        /// </summary>
        public bool AddConsumerProperty { get; set; } = true;

        /// <summary>
        /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Priority"/> property to <see cref="IThrottlingProvider"/>.
        /// </summary>
        public bool AddPriorityProperty { get; set; } = true;

        /// <summary>
        /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Method"/> property to <see cref="IThrottlingProvider"/>.
        /// </summary>
        public bool AddMethodProperty { get; set; } = true;

        /// <summary>
        /// If set to <c>true</c>, the middleware will pass <see cref="WellKnownThrottlingProperties.Url"/> property to <see cref="IThrottlingProvider"/>.
        /// </summary>
        public bool AddUrlProperty { get; set; }

        /// <summary>
        /// <para>If set to <c>true</c>, disables throttling entirely for web socket requests.</para>
        /// <para>Use <see cref="KestrelSettings.MaxConcurrentWebSocketConnections"/> to limit web socket parallelism.</para>
        /// </summary>
        public bool DisableForWebSockets { get; set; } = true;

        /// <summary>
        /// <para>Configuration of application's throttling metrics, enabled by default.</para>
        /// <para>Set this property to <c>null</c> to disable throttling metrics.</para>
        /// </summary>
        [CanBeNull]
        public ThrottlingMetricsOptions Metrics = new ThrottlingMetricsOptions();
    }
}
