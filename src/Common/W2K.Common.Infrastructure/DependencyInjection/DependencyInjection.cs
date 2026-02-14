#pragma warning disable CA1724 // Type names should not match namespaces
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Azure.Core;
using Azure.Identity;
using DFI.Common.Infrastructure.Settings;
using DFI.Common.Application.Cacheing;
using DFI.Common.Infrastructure.Cacheing;
using DFI.Common.Crypto;
using DFI.Common.Infrastructure.Crypto;
using DFI.Common.Infrastructure.AppInsights;
using DFI.Common.Application.ApiServices;
using DFI.Common.Infrastructure.ApiServices;
using DFI.Common.Infrastructure.Logging;
using Polly;
using DFI.Common.Infrastructure.AppConfig;
using SendGrid;
using DFI.Common.Application.Messaging;
using DFI.Common.Infrastructure.Messaging;
using Twilio.Clients;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using DFI.Common.Application.Storage;
using DFI.Common.Infrastructure.Storage;
using DFI.Common.Infrastructure.AzureAd;
using Microsoft.Graph;
using DFI.Common.Application.AzureAd;
using DFI.Common.Application.Session;
using DFI.Common.Infrastructure.Session;
using DFI.Common.Application.Auth;
using DFI.Common.Infrastructure.Auth;
using StackExchange.Redis;

namespace DFI.Common.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    private static readonly InfraSettings _settings = new();
    private static readonly CryptoSettings _cryptoSettings = new();

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env,
        ILogger logger)
    {
        _ = services.AddSingleton(configuration);
        _ = services.Configure<InfraSettings>(configuration.GetSection("InfraSettings"));
        _ = services.Configure<CryptoSettings>(configuration.GetSection("CryptoSettings"));
        _ = services.Configure<SessionStoreSettings>(configuration.GetSection("InfraSettings:SessionStoreSettings"));
        _ = services.Configure<HttpClientLoggingSettings>(configuration.GetSection("HttpClientLogging"));
        configuration.GetSection("InfraSettings").Bind(_settings);
        configuration.GetSection("CryptoSettings").Bind(_cryptoSettings);
        _ = services.Configure<AzureStorageSettings>(configuration.GetSection("InfraSettings:AzureStorageSettings"));

        // add Azure clients for logging https://docs.microsoft.com/en-us/dotnet/azure/sdk/logging
        services.AddAzureClientsCore();
        _ = services.AddAzureClients(env);

        // add Cache
        _ = services.AddCache(logger, env);

        // add secret repository
        if (env.IsDevelopment() || env.IsEnvironment("Test"))
        {
            _ = services.AddSingleton<ISecretRepository, LocalSecretRepository>();
        }

        // add crypto provider
        _ = services.AddSingleton<CryptoOptions, GetCryptoOptions>();
        _ = services.AddSingleton<ICryptoProvider, CryptoProvider>();

        // add application insights telemetry - https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
        _ = services.AddApplicationInsightsTelemetry(configuration);
        _ = services.AddApplicationInsightsTelemetryProcessor<AppInsightsProcessor>();
        _ = services.AddSingleton<Microsoft.ApplicationInsights.Extensibility.ITelemetryInitializer, AppInsightsInitializer>();

        // add Feature Management
        _ = services.AddFeatureManagement()
            .AddFeatureFilter<TargetingFilter>();
        _ = services.AddSingleton<ITargetingContextAccessor, OfficeTargetingContextAccessor>();

        // add HttpClient logging
        _ = services.AddTransient<HttpClientLoggingHandler>();

        // add API services
        _ = services.AddApiService(_settings.ApiServiceSettings);

        // add SendGrid email provider
        _ = services.AddSendGrid(logger);

        // add Twilio SMS provider
        _ = services.AddTwilio(logger);

        if (string.IsNullOrEmpty(_settings.AzureStorageSettings?.PrimaryUri))
        {
            logger.LogWarning("Azure Storage settings are not configured. Storage provider will not be available.");
        }
        else
        {
            _ = services.AddSingleton<IStorageProvider, AzureStorageProvider>();
        }

        // add Azure AD provider
        _ = services.AddAzureAD();

        // add session management
        _ = services.AddSession();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(
        this IApplicationBuilder app,
        IHostEnvironment env,
        Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration configuration)
    {
        if (env.IsDevelopment() || env.IsEnvironment("Test"))
        {
            // disable application insights telemetry in dev/test environments
            configuration.DisableTelemetry = true;
        }

        return app;
    }

    // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/extensions/Microsoft.Extensions.Azure/README.md
    private static IServiceCollection AddAzureClients(
        this IServiceCollection services,
        IHostEnvironment env)
    {
        services.AddAzureClients(x =>
            {
                if (!env.IsDevelopment() && !env.IsEnvironment("Test"))
                {
                    // add secret repo
                    _ = x.AddSecretClient(new Uri(_cryptoSettings.AzureKeyVaultUri));
                    _ = x.AddKeyClient(new Uri(_cryptoSettings.AzureKeyVaultUri));
                    _ = services.AddSingleton<SecretRepoOptions, GetSecretRepoOptions>();
                    _ = services.AddSingleton<ISecretRepository, SecretRepository>();
                }

                // Configures environment credential to be used by default for all clients that require TokenCredential and doesn't override it on per registration level
                _ = x.UseCredential(new DefaultAzureCredential());

                // Configure global retry mode
                _ = x.ConfigureDefaults(opts =>
                    {
                        opts.Retry.MaxRetries = 5;
                        opts.Retry.Mode = RetryMode.Exponential;
                    });
            });

        return services;
    }

    private static IServiceCollection AddApiService(
        this IServiceCollection services,
        ApiServiceSettings settings)
    {
        foreach (var key in settings.Apis.Keys)
        {
            _ = services.AddHttpClient(key, x => x.BaseAddress = settings.GetBaseUri(key))
                .AddHttpMessageHandler<HttpClientLoggingHandler>()
                .AddTransientHttpErrorPolicy(x =>
                    x.WaitAndRetryAsync(settings.GetRetryCount(key), _ => settings.GetRetryDelay(key)));
        }
        return services.AddTransient<IApiService, ApiService>();
    }

    private static IServiceCollection AddSendGrid(this IServiceCollection services, ILogger logger)
    {
        if (string.IsNullOrEmpty(_settings.SendGridApiKey))
        {
            logger.LogWarning("SendGrid API Key is not configured. Email provider will not be available.");
        }
        else
        {
            _ = services.AddSingleton<ISendGridClient>(new SendGridClient(_settings.SendGridApiKey));
            _ = services.AddSingleton<IEmailProvider, SendGridEmailProvider>();
        }
        return services;
    }

    private static IServiceCollection AddTwilio(this IServiceCollection services, ILogger logger)
    {
        if (string.IsNullOrEmpty(_settings.TwilioSettings.AccountSid)
            || string.IsNullOrEmpty(_settings.TwilioSettings.AuthToken))
        {
            logger.LogWarning("Twilio settings are not configured. SMS provider will not be available.");
        }
        else
        {
            _ = services.AddHttpClient<ITwilioRestClient, TwilioClient>().AddHttpMessageHandler<HttpClientLoggingHandler>();
            _ = services.AddSingleton<TwilioSettings, GetTwilioSettings>();
            _ = services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
        }
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, ILogger logger, IHostEnvironment env)
    {
        if (string.IsNullOrEmpty(_settings.RedisCacheSettings.ConnectionString))
        {
            logger.LogWarning("RedisCacheSettings settings are not configured. Will use default in-memory cache.");
            _ = services.AddDistributedMemoryCache();
            _ = services.AddSingleton<ICache, Cache>();
        }
        else
        {
            var redisConnectionString = _settings.RedisCacheSettings.ConnectionString;

            // Parse the connection string to get the host and port
            var parts = redisConnectionString.Split(':');
            var endPoint = parts[0];
            var port = parts.Length > 1 ? int.Parse(parts[1]) : 6380; // Default SSL port for Azure Redis 6380

            var configurationOptions = new ConfigurationOptions
            {
                EndPoints = { { endPoint, port } },
                Ssl = !env.IsDevelopment(),
                AbortOnConnectFail = false,
                ConnectTimeout = _settings.RedisCacheSettings.ConnectTimeoutMS,
                SyncTimeout = _settings.RedisCacheSettings.SyncTimeoutMS,
                AsyncTimeout = _settings.RedisCacheSettings.AsyncTimeoutMS
            };

            var credential = new DefaultAzureCredential();
            configurationOptions.ConfigureForAzureWithTokenCredentialAsync(credential).GetAwaiter().GetResult();

            // Register ConnectionMultiplexer as a singleton with proper authentication
            _ = services.AddSingleton<IConnectionMultiplexer>(x =>
                {
                    var connection = ConnectionMultiplexer.Connect(configurationOptions);
                    logger.LogInformation("Redis ConnectionMultiplexer created successfully for endpoint {EndPoint}", endPoint);
                    return connection;
                });

            _ = services.AddStackExchangeRedisCache(x =>
                {
                    x.ConfigurationOptions = configurationOptions;
                    x.InstanceName = _settings.RedisCacheSettings.InstanceName;
                });

            _ = services.AddSingleton<ICache, RedisCache>();

            // Health check using the same configuration options
            _ = services.AddHealthChecks().AddRedis(
                configurationOptions.ToString(),
                "redis-check",
                HealthStatus.Unhealthy);
        }

        return services;
    }

    private static IServiceCollection AddAzureAD(this IServiceCollection services)
    {
        if (!string.IsNullOrEmpty(_settings.AzureAD.TenantId) && !string.IsNullOrEmpty(_settings.AzureAD.ClientId) && !string.IsNullOrEmpty(_settings.AzureAD.ClientSecret))
        {
            var scopes = new[] { AzureADConstants.GraphScopes };
            _ = services.AddSingleton(_settings.AzureAD);
            _ = services.AddSingleton(new GraphServiceClient(
                new ClientSecretCredential(
                _settings.AzureAD.TenantId,
                _settings.AzureAD.ClientId,
                _settings.AzureAD.ClientSecret,
                new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                }),
                scopes));

            _ = services.AddSingleton<IGraphService, GraphService>();
            _ = services.AddSingleton<IAzureADProvider>(x => new AzureADProvider(
                                                                x.GetRequiredService<ILogger<AzureADProvider>>(),
                                                                x.GetRequiredService<IGraphService>(),
                                                                x.GetRequiredService<ICryptoProvider>(),
                                                                x.GetRequiredService<AzureADSettings>()));
        }
        return services;
    }

    private static IServiceCollection AddSession(this IServiceCollection services)
    {
        _ = services.AddSingleton<ISessionStore, SessionStore>();
        _ = services.AddSingleton<IThrottlingStore, ThrottlingStore>();
        return services;
    }

}
#pragma warning restore CA1724 // Type names should not match namespaces
