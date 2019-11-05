﻿using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.AspNetCore.Helpers;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Microsoft;

namespace Vostok.Hosting.AspNetCore
{
    [PublicAPI]
    public abstract class VostokAspNetCoreApplication : IVostokApplication
    {
        private IApplicationLifetime lifetime;
        private ILog log;
        private IWebHost webHost;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            var builder = WebHost.CreateDefaultBuilder()
                .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
                .ConfigureLogging(loggingBuilder => loggingBuilder.AddProvider(new VostokLoggerProvider(environment.Log)))
                .ConfigureUrl(environment)
                .AddStartupFilter(new UrlPathStartupFilter(environment));

            builder = ConfigureWebHostBuilder(builder, environment);

            webHost = builder.Build();

            await StartWebHost(environment).ConfigureAwait(false);

            await WarmUpAsync(environment).ConfigureAwait(false);
        }

        public Task RunAsync(IVostokHostingEnvironment environment)
        {
            RunWebHost();

            return Task.CompletedTask;
        }

        public abstract IWebHostBuilder ConfigureWebHostBuilder(IWebHostBuilder builder, IVostokHostingEnvironment environment);

        public virtual Task WarmUpAsync(IVostokHostingEnvironment environment)
            => Task.CompletedTask;

        private async Task StartWebHost(IVostokHostingEnvironment environment)
        {
            log = environment.Log.ForContext<VostokAspNetCoreApplication>();

            lifetime = (IApplicationLifetime)webHost.Services.GetService(typeof(IApplicationLifetime));

            environment.ShutdownToken.Register(
                () => webHost
                    .StopAsync()
                    .ContinueWith(t => log.Error(t.Exception), TaskContinuationOptions.OnlyOnFaulted));

            log.Info("Starting WebHost.");
            await webHost.StartAsync().ConfigureAwait(false);
            lifetime.ApplicationStarted.WaitHandle.WaitOne();
            log.Info("WebHost started.");
        }

        private void RunWebHost()
        {
            lifetime.ApplicationStopping.WaitHandle.WaitOne();
            log.Info("Stopping WebHost.");

            lifetime.ApplicationStopped.WaitHandle.WaitOne();
            log.Info("WebHost stopped.");

            webHost.Dispose();
        }
    }
}