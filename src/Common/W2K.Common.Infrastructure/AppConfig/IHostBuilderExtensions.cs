using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using W2K.Common.Infrastructure.Settings;
using W2K.Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Infrastructure.AppConfig;

public static class IHostBuilderExtensions
{
    public static IHostBuilder CreateHostBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.PublicMethods)] TStartup>(this string[] args)
            where TStartup : class
    {
        var hostBuilder = Host.CreateDefaultBuilder(args);
        _ = hostBuilder.ConfigureAzureLogging();
        _ = hostBuilder.ConfigureWebHost<TStartup>();
        return hostBuilder;
    }

    public static IHostBuilder ConfigureAzureLogging(this IHostBuilder builder)
    {
        _ = builder.ConfigureLogging((ctx, logging) =>
            {
                var env = ctx.HostingEnvironment;
                env.AddLogging(logging);
            });

        return builder;
    }

    public static IHostBuilder ConfigureWebHost<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.PublicMethods)] TStartup>(this IHostBuilder builder) where TStartup : class
    {
        _ = builder.ConfigureWebHostDefaults(
            x =>
            {
                _ = x.ConfigureKestrel(
                        serverOptions =>
                        {
                            // Set properties and call methods on options
                            serverOptions.AddServerHeader = false;
                        })
                    .UseStartup<TStartup>();
            });
        return builder;
    }

    public static void AddLogging(this IHostEnvironment env, ILoggingBuilder loggingBuilder)
    {
        if (env.IsDevelopment() || env.IsEnvironment("Test"))
        {
            _ = loggingBuilder.AddDebug();
            _ = loggingBuilder.AddConsole();
        }
        else
        {
            _ = loggingBuilder.AddAzureWebAppDiagnostics();
            _ = loggingBuilder.AddApplicationInsights();
        }
    }

    public static IHostBuilder AddAppConfig(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var config = configBuilder.Build();
                var env = context.HostingEnvironment;
                var uri = config?.GetSection(nameof(InfraSettings))?.GetValue<string>(nameof(InfraSettings.AzureAppConfigUri));
                if (config is not null && !string.IsNullOrEmpty(uri))
                {
                    var sentinelKey = config.GetSection(nameof(InfraSettings)).GetValue<string>(nameof(InfraSettings.AzureAppConfigSentinelKey));
                    var cacheDuration = config.GetSection(nameof(InfraSettings)).GetValue<TimeSpan?>(nameof(InfraSettings.AzureAppConfigCachePeriod));
                    var label = env.EnvironmentName.ToUpperInvariant();
                    _ = AddAppConfig(configBuilder, uri, sentinelKey, cacheDuration, label);
                }
            });
    }

    public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, IConfiguration? config, string? label = null)
    {
        var section = config?.GetSection(nameof(InfraSettings));
        var uri = section?.GetValue<string>(nameof(InfraSettings.AzureAppConfigUri));
        if (section is not null && !string.IsNullOrEmpty(uri))
        {
            var sentinelKey = section.GetValue<string>(nameof(InfraSettings.AzureAppConfigSentinelKey));
            var cacheDuration = section.GetValue<TimeSpan?>(nameof(InfraSettings.AzureAppConfigCachePeriod));
            _ = builder.AddAppConfig(uri, sentinelKey, cacheDuration, label);
        }
        return builder;
    }

    public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, string uri, string? sentinelKey, TimeSpan? cacheDuration, string? label = null)
    {
        var creds = new DefaultAzureCredential();
        return builder.AddAzureAppConfiguration(x =>
            {
                _ = x.Connect(uri.GetUris(), creds)
                    .Select(KeyFilter.Any, label ?? LabelFilter.Null)
                    .UseFeatureFlags(ff => ff.Label = label ?? LabelFilter.Null)
                    .ConfigureKeyVault(kv => kv.SetCredential(creds));
                if (!string.IsNullOrEmpty(sentinelKey) || cacheDuration.HasValue)
                {
                    _ = x.ConfigureRefresh(refresh =>
                        {
                            if (!string.IsNullOrEmpty(sentinelKey))
                            {
                                _ = refresh.Register(key: sentinelKey, refreshAll: true, label: label ?? LabelFilter.Null);
                            }
                            if (cacheDuration.HasValue)
                            {
                                _ = refresh.SetRefreshInterval(cacheDuration.Value);
                            }
                        });
                }
            });
    }
}
