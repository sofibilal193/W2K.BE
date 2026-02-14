using W2K.Common.Application.DependencyInjection;
using W2K.Common.Infrastructure.AppConfig;
using W2K.Common.Infrastructure.DependencyInjection;
using W2K.Common.Persistence.DependencyInjection;
using W2K.Config.DependencyInjection;
using W2K.Files.DependencyInjection;
using W2K.Funding.DependencyInjection;
using W2K.Identity.DependencyInjection;
using W2K.Lenders.DependencyInjection;
using W2K.Loans.DependencyInjection;
using W2K.Messaging.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using W2K.WebApp;

namespace W2K.Mono;

public class Startup
{
    private IServiceCollection _services = new ServiceCollection();
    private readonly ILoggerFactory _loggerFactory;

    public IConfiguration Configuration { get; }
    public IHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
        _loggerFactory = LoggerFactory.Create(x => Environment.AddLogging(x));
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var logger = _loggerFactory.CreateLogger<Startup>();
        _ = services.AddApplication(Configuration, Environment, logger);
        _ = services.AddPersistenceEvents(Configuration, logger, WebAppConstants.ApplicationName);
        _ = services.AddInfrastructure(Configuration, Environment, logger);
        _ = services.AddIdentityApi(Configuration);
        _ = services.AddFilesApi(Configuration);
        _ = services.AddConfigApi(Configuration);

        // add health checks
        _ = services.AddAppHealthChecks(true);
        _services = services;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(
        IApplicationBuilder app,
        IConfiguration config,
        IHostEnvironment env,
        TelemetryConfiguration telemetryConfiguration)
    {
        var enableHttpLogging = config.GetSection(WebAppConstants.ApplicationName).GetValue<bool>("EnableHttpLogging");
        _ = app.UseApplication(env, _services, enableHttpLogging);
        _ = app.UsePersistence(env);
        _ = app.UseInfrastructure(env, telemetryConfiguration);
        _ = app.UseIdentityApi();
        _ = app.UseFilesApi();
        _ = app.UseConfigApi();
        _ = app.UseLoansApi();
        _ = app.UseFundingApi();
        _ = app.UseMessagingApi();
        _ = app.UseLendersApi();
    }
}

