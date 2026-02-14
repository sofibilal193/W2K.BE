#pragma warning disable CA1724 // Type names should not match namespaces
using FluentValidation;
using System.Reflection;
using W2K.Common.Application.Cacheing;
using W2K.Common.Persistence.DependencyInjection;
using W2K.Common.Persistence.Settings;
using W2K.Identity.Persistence.Context;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using W2K.Identity.Application.Queries;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using W2K.Identity.Auth;
using Microsoft.AspNetCore.Builder;
using W2K.Common.Application.DependencyInjection;
using W2K.Identity.Settings;

namespace W2K.Identity.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add MediatR for currently executing assembly
        _ = services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // add AutoMapper
        _ = services.AddAutoMapper(x => x.AddProfile<Application.Mapping.MappingProfile>());

        var appSection = configuration.GetSection(IdentityConstants.ApplicationName);
        _ = services.Configure<PersistenceSettings>(IdentityConstants.ApplicationName, appSection.GetSection("PersistenceSettings"));
        _ = services.Configure<ServiceSettings>(appSection.GetSection("ServiceSettings"));

        // add persistence services
        _ = services.AddPersistence<IdentityDbContext>(configuration, IdentityConstants.ApplicationName, Assembly.GetExecutingAssembly().FullName);

        _ = services.AddScoped<IOfficeRepository, OfficeRepository>();
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IRoleRepository, RoleRepository>();
        _ = services.AddScoped<IPermissionRepository, PermissionRepository>();
        _ = services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();
        _ = services.AddScoped<IOfficeUserRepository, OfficeUserRepository>();
        _ = services.AddScoped<ISessionLogsRepository, SessionLogsRepository>();

        // ICachePolicy discovery and registration
        // https://andrewlock.net/using-scrutor-to-automatically-register-your-services-with-the-asp-net-core-di-container/
#pragma warning disable T0028 // Internal Styling Rule T0028
        _ = services.Scan(x => x
                                .FromAssemblies(Assembly.GetExecutingAssembly())
                                .AddClasses(classes => classes.AssignableTo(typeof(ICachePolicy<,>)))
                                .AsImplementedInterfaces()
                                .WithTransientLifetime());
#pragma warning restore T0028 // Internal Styling Rule T0028

        // add FluentValidation for calling Assembly
        _ = services.AddValidatorsFromAssemblyContaining<GetLoginUserInfoQueryValidator>();

        // add jwt options to intercept token validation event
        _ = services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

        // add health checks
        _ = services.AddAppHealthChecks();

        return services;
    }

    public static IApplicationBuilder UseIdentityApi(this IApplicationBuilder app)
    {
        return app;
    }
}
#pragma warning restore CA1724 // Type names should not match namespaces
