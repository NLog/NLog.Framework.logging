﻿using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;

namespace NLog.Extensions.Hosting
{
    /// <summary>
    ///     Helpers for IHostbuilder, netcore 2.1
    /// </summary>
    public static class ConfigureExtensions
    {
        /// <summary>
        ///     Enable and configure NLog as a logging provider for buildable generic host (.NET Core 2.1+).
        ///     Can be used in discrete containers as well.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>IHostBuilder for chaining</returns>
        public static IHostBuilder UseNLog(this IHostBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            return builder.UseNLog(null);
        }

        /// <summary>
        ///     Enable and configure NLog as a logging provider for buildable generic host (.NET Core 2.1+).
        ///     Can be used in discrete containers as well.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options">NLogProviderOptions object to configure NLog behavior</param>
        /// <returns>IHostBuilder for chaining</returns>
        public static IHostBuilder UseNLog(this IHostBuilder builder, NLogProviderOptions options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((hostbuilder, services) =>
            {
                ConfigurationItemFactory.Default.RegisterItemsFromAssembly(typeof(ConfigureExtensions).GetTypeInfo()
                    .Assembly);

                services.AddSingleton<ILoggerProvider>(serviceProvider =>
                {
                    var provider = new NLogLoggerProvider(options ?? new NLogProviderOptions());
                    if (hostbuilder.Configuration != null)
                    {
                        // TODO ConfigSettingLayoutRenderer.DefaultConfiguration = hostbuilder.Configuration;
                        if (options == null)
                            provider.ConfigureNLogProvider(hostbuilder.Configuration?.GetSection("Logging:NLog"));
                    }
                    return provider;
                });
            });

            return builder;
        }
    }
}