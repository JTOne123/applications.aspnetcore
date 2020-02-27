﻿using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Vostok.Applications.AspNetCore.Builders;
using Vostok.Applications.AspNetCore.Helpers;
using Vostok.Hosting.Abstractions;
using Vostok.Logging.Abstractions;

namespace Vostok.Applications.AspNetCore
{
    /// <summary>
    /// <para><see cref="VostokNetCoreApplication"/> is the abstract class developers inherit from in order to create Vostok-compatible NetCore applications.</para>
    /// <para>Implement <see cref="Setup"/> method to configure <see cref="IHostBuilder"/> (see <see cref="IVostokNetCoreApplicationBuilder"/>).</para>
    /// </summary>
    [PublicAPI]
    public abstract class VostokNetCoreApplication : IVostokApplication, IDisposable
    {
        private volatile HostManager manager;

        public async Task InitializeAsync(IVostokHostingEnvironment environment)
        {
            var log = environment.Log.ForContext<VostokNetCoreApplication>();

            var builder = new VostokNetCoreApplicationBuilder(environment);

            Setup(builder, environment);

            manager = new HostManager(builder.Build(), log);

            await manager.StartHostAsync(environment).ConfigureAwait(false);
        }

        public Task RunAsync(IVostokHostingEnvironment environment) =>
            manager.RunHostAsync();

        /// <summary>
        /// Implement this method to configure <see cref="IHostBuilder"/> and customize built-in Vostok middleware components.
        /// </summary>
        public abstract void Setup([NotNull] IVostokNetCoreApplicationBuilder builder, [NotNull] IVostokHostingEnvironment environment);

        public void Dispose()
            => manager?.Dispose();
    }
}