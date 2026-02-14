using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using DFI.Common.Persistence.Settings;
using DFI.Common.Persistence.Extensions;

namespace DFI.Common.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "Abstract class does not need unit tests.")]
public abstract class DesignTimeDbContextFactoryBase<TContext> :
    IDesignTimeDbContextFactory<TContext> where TContext : BaseDbContext<TContext>
{
    public abstract TContext CreateNewInstance(
         DbContextOptions<TContext> options,
         ILoggerFactory loggerFactory,
         IMediator mediator,
         IOptions<PersistenceSettings> settings);

    public virtual string AspNetCoreEnvironment => "ASPNETCORE_ENVIRONMENT";

    public TContext CreateDbContext(string[] args)
    {
        string? appName, connectionStringName;
        if (args.Length > 0)
        {
            appName = args[0];
            connectionStringName = args.Length > 1 ? args[1] : PersistenceConstants.DefaultConnectionName;
        }
        else
        {
            appName = PersistenceConstants.DefaultAppName;
            connectionStringName = PersistenceConstants.DefaultConnectionName;
        }
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable(AspNetCoreEnvironment)}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddLogging(
                x =>
                {
                    _ = x.AddConfiguration(configuration.GetSection("Logging"));
                    _ = x.AddConsole();
                    _ = x.AddDebug();
                })
            .BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<ILoggerFactory>();

        var settings = configuration.GetPersistenceSettings(appName) ?? throw new ArgumentException($"Persistence settings for '{appName}' with connection string '{connectionStringName}' not found.");
        var optionSettings = Options.Create(settings);
        var connectionString = configuration.GetConnectionString(appName, connectionStringName) ?? settings.DbConnectionString;
        var useAzureAd = settings.UseAzureAdAuth;
        if (!string.IsNullOrEmpty(connectionString))
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            _ = optionsBuilder.UseSqlServer(connectionString);
            if (useAzureAd)
            {
                // Add Azure AD authentication interceptor
                _ = optionsBuilder.AddInterceptors(new AzureAdAuthDbConnectionInterceptor());
            }
            return CreateNewInstance(optionsBuilder.Options, factory, new NoMediator(), optionSettings);
        }

        throw new ArgumentException($"Connection string '{connectionString}' is null or empty.");
    }
}
