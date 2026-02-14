#pragma warning disable CA1724 // Type names should not match namespaces
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Context;
using W2K.Common.Events;
using W2K.Common.Persistence.Events;
using W2K.Common.Persistence.Interfaces;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Persistence.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceEvents(
        this IServiceCollection services,
        IConfiguration configuration,
        ILogger logger,
        string appName)
    {
        var settings = configuration.GetPersistenceSettings(appName);
        if (string.IsNullOrEmpty(settings?.DbConnectionString))
        {
            logger.LogWarning("Persistence settings for '{AppName}' not found. Event logging will not be configured.", appName);
        }
        else
        {
            _ = services.AddDbContext<EventDbContext>(x =>
                {
                    if (settings.UseAzureAdAuth)
                    {
                        _ = x.UseAzureSql(
                            settings.DbConnectionString,
                            azureSqlOptionsAction: sqlOptions =>
                            {
                                _ = sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                                _ = sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: settings.MaxRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                    errorNumbersToAdd: null);
                            });
                        _ = x.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
                    }
                    else
                    {
                        _ = x.UseSqlServer(
                            settings.DbConnectionString,
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                _ = sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                                _ = sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: settings.MaxRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                    errorNumbersToAdd: null);
                            });
                    }
                });

            _ = services.AddHealthChecks().AddDbContextCheck<EventDbContext>();
            _ = services.AddSingleton<IEventLogSink, EventLogSink<EventDbContext>>();
        }

        return services;
    }

    public static IServiceCollection AddPersistence<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string appName,
        string? migrationsAssemblyName) where TContext : BaseDbContext<TContext>
    {
        var settings = configuration.GetPersistenceSettings(appName);
        if (!string.IsNullOrEmpty(settings?.DbConnectionString))
        {
            _ = services.AddDbContext<TContext>(x =>
                {
                    if (settings.UseAzureAdAuth)
                    {
                        _ = x.UseAzureSql(
                            settings.DbConnectionString,
                            azureSqlOptionsAction: sqlOptions =>
                            {
                                _ = sqlOptions.MigrationsAssembly(migrationsAssemblyName);
                                _ = sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: settings.MaxRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                    errorNumbersToAdd: null);
                            });
                        _ = x.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
                    }
                    else
                    {
                        _ = x.UseSqlServer(
                            settings.DbConnectionString,
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                _ = sqlOptions.MigrationsAssembly(migrationsAssemblyName);
                                _ = sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: settings.MaxRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(settings.MaxRetryDelaySeconds),
                                    errorNumbersToAdd: null);
                            });
                    }
                    if (settings.EnableSensitiveDataLogging)
                    {
                        _ = x.EnableSensitiveDataLogging();
                    }
                });
            _ = services.AddScoped<IDbContext>(x => x.GetRequiredService<TContext>());
            _ = services.AddHealthChecks().AddDbContextCheck<TContext>();
        }
        _ = services.AddDatabaseDeveloperPageExceptionFilter();
        return services;
    }

    public static IApplicationBuilder UsePersistence(this IApplicationBuilder app, IHostEnvironment env)
    {
        if (!env.IsProduction())
        {
            _ = app.UseMigrationsEndPoint();
        }
        return app;
    }
}
#pragma warning restore CA1724 // Type names should not match namespaces
