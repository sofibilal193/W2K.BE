#pragma warning disable CA1724 // Type names should not match namespaces
using System.Reflection;
using W2K.Common.Application.Cacheing;
using W2K.Common.Persistence.DependencyInjection;
using W2K.Common.Persistence.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using W2K.Config.Persistence.Context;
using W2K.Config.Repositories;
using W2K.Common.Application.DependencyInjection;

namespace W2K.Config.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddConfigApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Add MediatR for currently executing assembly
        _ = services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // add AutoMapper
        _ = services.AddAutoMapper(x => x.AddProfile<Application.Mapping.MappingProfile>());

        var appSection = configuration.GetSection(ConfigConstants.ApplicationName);
        _ = services.Configure<PersistenceSettings>(ConfigConstants.ApplicationName, appSection.GetSection("PersistenceSettings"));

        // add persistence services
        _ = services.AddPersistence<ConfigDbContext>(configuration, ConfigConstants.ApplicationName, Assembly.GetExecutingAssembly().FullName);

        _ = services.AddScoped<IConfigRepository, ConfigRepository>();
        _ = services.AddScoped<IOfficeConfigFieldRepository, OfficeConfigFieldRepository>();
        _ = services.AddScoped<IOfficeConfigFieldValueRepository, OfficeConfigFieldValueRepository>();
        _ = services.AddScoped<IConfigUnitOfWork, ConfigUnitOfWork>();

        // ICachePolicy discovery and registration
        // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
#pragma warning disable T0028 // Internal Styling Rule T0028
        _ = services.Scan(x => x
                                .FromAssemblies(Assembly.GetExecutingAssembly())
                                .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)))
                                .AsImplementedInterfaces()
                                .WithTransientLifetime());
#pragma warning restore T0028 // Internal Styling Rule T0028

        // add health checks
        _ = services.AddAppHealthChecks();

        return services;
    }

    public static IApplicationBuilder UseConfigApi(this IApplicationBuilder app)
    {
        return app;
    }
}
#pragma warning restore CA1724 // Type names should not match namespaces
