#pragma warning disable CA1851 // Possible multiple enumerations of IEnumerable collection
#pragma warning disable CA1724 // Type names should not match namespaces

using System.Globalization;
using System.Text.Json.Serialization;
using Asp.Versioning;
using W2K.Common.Application.Auth;
using W2K.Common.Application.BackgroundServices;
using W2K.Common.Application.Behaviors;
using W2K.Common.Application.Extensions;
using W2K.Common.Application.Filters;
using W2K.Common.Application.Identity;
using W2K.Common.Application.Settings;
using W2K.Common.Application.Validations;
using W2K.Common.Identity;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using Microsoft.AspNetCore.Http;
using W2K.Common.Crypto;
using W2K.Common.Application.Crypto;
using W2K.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.ApiClients;

namespace W2K.Common.Application.DependencyInjection;

public static class DependencyInjection
{
    private const string _xForwardedPathBaseHeader = "X-Forwarded-PathBase";
    private static readonly AppSettings _settings = new();

    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env,
        ILogger logger)
    {
        _ = services.AddSingleton(configuration);
        _ = services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        configuration.GetSection("AppSettings").Bind(_settings);

        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
        AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(_settings.RegexTimeoutSeconds));

        _ = services.AddHttpContextAccessor();

        if (_settings.MaxRequestFileSizeInBytes.HasValue)
        {
            _ = services.Configure<KestrelServerOptions>(x =>
                {
                    x.Limits.MaxRequestBodySize = _settings.MaxRequestFileSizeInBytes;
                });
        }

        _ = services
            .AddControllers(x => x.Filters.Add<HttpGlobalExceptionFilter>())
            .ConfigureApiBehaviorOptions(x => x.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

        _ = services.AddScoped<ICurrentUser, HttpCurrentUser>();

        // Background service dependencies
        _ = services.Configure<BackgroundServiceSettings>(configuration.GetSection("BackgroundServiceSettings"));

        // https://andrewlock.net/handling-web-api-exceptions-with-problemdetails-middleware/
        _ = services.AddProblemDetails(x =>
            {
                // Control when an exception is included
                x.IncludeExceptionDetails = (_, _) => env.IsDevelopment() || env.IsTest();
            });
        ValidatorOptions.Global.LanguageManager = new ValidationLanguageManager();

        // add cors policies
        _ = services.AddCorsPolicies(_settings, logger);

        // add auth
        _ = services.AddAuthentication(configuration, logger);
        _ = services.AddAuthorization();

#pragma warning disable T0028 // Internal Styling Rule T0028 for commas in multi-line statements
        // add Mediatr
        _ = services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ValidationBehavior<,>).Assembly));
        _ = services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CachePipelineBehavior<,>).Assembly));

        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachePipelineBehavior<,>));

        // add validators from W2K.Common.Application
        _ = services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
#pragma warning restore T0028 // Internal Styling Rule T0028

        // configure cookie options
        _ = services.Configure<CookiePolicyOptions>(x =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                x.CheckConsentNeeded = _ => true;
                x.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

        // add HTTP logging
        _ = services.AddHttpLogging(x =>
            {
                x.LoggingFields = HttpLoggingFields.All;
                // log header values needed to debug reverse proxy issues
                _ = x.RequestHeaders.Add("X-Forwarded-Host");
                _ = x.RequestHeaders.Add("X-ORIGINAL-HOST");
                _ = x.RequestHeaders.Add(_xForwardedPathBaseHeader);
                _ = x.RequestHeaders.Add("X-Original-URL");
                _ = x.RequestHeaders.Add("X-Azure-ClientIP");
                _ = x.RequestHeaders.Add("X-Azure-SocketIP");
                _ = x.RequestHeaders.Add("X-Forwarded-For");
                _ = x.RequestHeaders.Add("X-Original-For");
                _ = x.RequestHeaders.Add("Client-IP");
                _ = x.RequestHeaders.Add(AuthConstants.ClientIpHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.AzureClientIpHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.RemoteAddressHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.ApiKeyHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.InternalServiceAuthKeyHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.SessionIdHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.FingerPrintHeaderName);
                _ = x.RequestHeaders.Add(AuthConstants.WebhookApiAuthScheme);
            });

        // api versioning - https://code-maze.com/aspnetcore-api-versioning/
        var apiVersioningBuilder = services.AddApiVersioning(x =>
            {
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new Asp.Versioning.ApiVersion(
                    _settings.ApiVersionSettings.DefaultVersion.MajorVersion,
                    _settings.ApiVersionSettings.DefaultVersion.MinorVersion);
                x.ReportApiVersions = true;
                x.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("x-api-version"),
                    new MediaTypeApiVersionReader("ver"),
                    new UrlSegmentApiVersionReader());
            });
        _ = apiVersioningBuilder.AddApiExplorer(
            x =>
            {
                x.GroupNameFormat = "'v'VVV";
                x.SubstituteApiVersionInUrl = true;
            });

        // add NSwag Open Api specification - https://blog.rsuter.com/versioned-aspnetcore-apis-with-openapi-generation-and-azure-api-management/
        if (_settings.ApiVersionSettings.Versions is not null)
        {
            foreach (var version in _settings.ApiVersionSettings.Versions)
            {
                _ = services.AddOpenApiDocument(x =>
                    {
                        x.Title = _settings.OpenApiSettings.Title;
                        x.Description = _settings.OpenApiSettings.Description ?? "";

                        x.DocumentName = "v" + version.MajorVersion;
                        x.ApiGroupNames = ["v" + version.MajorVersion];
                        x.Version = version.MajorVersion + "." + version.MinorVersion;
                        x.SchemaSettings.SchemaNameGenerator = new ApiSchemaNameGenerator();
                        x.SchemaSettings.GenerateXmlObjects = true;

                        _ = x.AddSecurity("JWT", new OpenApiSecurityScheme
                        {
                            // Suggested by GitHub Copilot
                            Type = OpenApiSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Description = "Type into the textbox: Bearer {your JWT token}."
                        });

                        x.OperationProcessors.Add(
                            new AspNetCoreOperationSecurityScopeProcessor("JWT"));

                        if (_settings.OpenApiSettings.ShowSessionIdHeader)
                        {
                            x.OperationProcessors.Add(new AddSessionIdHeaderProcessor());
                        }
                    });
            }
        }

        // add client crypto provider
        _ = services.AddSingleton<IClientCryptoProvider, ClientCryptoProvider>();

        // add API clients
        _ = services.AddApiClients();

        return services;
    }

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation(
            "Using {AuthType} authentication with ClientId: {ClientId}, TenantId: {TenantId}, AuthInstance: {AuthInstance}, Domain: {Domain}",
            _settings.AuthSettings.Type,
            _settings.AuthSettings.ClientId,
            _settings.AuthSettings.TenantId,
            _settings.AuthSettings.Instance,
            _settings.AuthSettings.Domain);

        _ = _settings.AuthSettings.Type == ApiAuthType.JWT || string.IsNullOrEmpty(_settings.AuthSettings.ClientId)
            ? services.AddAuthentication(x =>
                    {
                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtAuth(logger)
            : services.AddAuthentication(x =>
                    {
                        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddApiAuth(configuration, logger);

        // add basic auth
        _ = services.AddAuthentication(AuthConstants.BasicAuthScheme)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthConstants.BasicAuthScheme, null);

        // add session auth
        _ = services.AddAuthentication(AuthConstants.SessionAuthScheme)
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(AuthConstants.SessionAuthScheme, null);

        // add API key auth
        _ = services.AddAuthentication(AuthConstants.ApiKeyAuthScheme)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthConstants.ApiKeyAuthScheme, null);

        // add Webhook API key auth
        _ = services.AddAuthentication(AuthConstants.WebhookApiAuthScheme)
            .AddScheme<AuthenticationSchemeOptions, WebHookApiKeyAuthenticationHandler>(AuthConstants.WebhookApiAuthScheme, null);

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>());
#pragma warning restore S125 // Sections of code should not be commented out
        return services;
    }

    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, bool registerSelfCheck = false)
    {
        var hcBuilder = services.AddHealthChecks();
        if (registerSelfCheck)
        {
            _ = hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
        }

        var sb = services.BuildServiceProvider();
        var hcs = Assembly.GetCallingAssembly()?.GetTypes()
            .Where(x => x.GetInterfaces().Contains(typeof(IHealthCheck)))
            .Select(x => ActivatorUtilities.CreateInstance(sb, x) as IHealthCheck);
        if (hcs?.Any() == true)
        {
            var tags = new[] { "custom" };
            foreach (var hc in hcs)
            {
                _ = hcBuilder.AddCheck(
                    hc!.GetType().Name.ToUpperInvariant().Replace("HEALTHCHECK", "", StringComparison.OrdinalIgnoreCase),
                    hc,
                    HealthStatus.Degraded,
                    tags);
            }
        }
        return services;
    }

    public static IApplicationBuilder UseApplication(this IApplicationBuilder app, IHostEnvironment env, IServiceCollection services, bool enableHttpLogging)
    {
        if (env.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
        }

        JsonSerializerOptionsExtensions.SetServiceProvider(app.ApplicationServices.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions, app.ApplicationServices);

        _ = app.UseProblemDetails();

        if (!env.IsDevelopment() && !env.IsEnvironment("Test"))
        {
            // when in azure, ensure headers are forwarded to prevent too many multiple redirects issue
            // https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.2&tabs=visual-studio
            _ = app.UseXForwardedHeaders();
        }

        if (enableHttpLogging)
        {
            _ = app.UseHttpLogging();
        }

        if (!env.IsProduction())
        {
            _ = app.UseDeveloperExceptionPage();
            RegisteredServicesPage(app, services);
        }

        // use NSwag OpenApi
        _ = app.UseSwaggerWithReverseProxySupport();

        _ = app.UseRouting();

        // UseCors with default policy.
        _ = app.UseCors();

        // use authentication & authorization
        _ = app.UseAuthentication();
        _ = app.UseAuthorization();

        _ = app.UseEndpoints(x =>
            {
                _ = x.MapDefaultControllerRoute();
                _ = x.MapControllers();
                _ = x.MapAppHealthChecks();
            });

        return app;
    }

    private static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder app)
    {
        _ = app.MapHealthChecks("/hc", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        _ = app.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = x => x.Name.Contains("self", StringComparison.OrdinalIgnoreCase)
        });
        _ = app.MapHealthChecks("/hc-custom", new HealthCheckOptions
        {
            Predicate = x => x.Tags.Contains("custom"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }

    private static IApplicationBuilder UseXForwardedHeaders(this IApplicationBuilder app)
    {
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        };

        // Only loopback proxies are allowed by default.
        // Clear that restriction because forwarders are being enabled by explicit configuration.
        // https://stackoverflow.com/a/56469499/5358985
        // KnownIPNetworks replaces deprecated KnownNetworks property in .NET 10
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();

        _ = app.UseForwardedHeaders(options);

        _ = app.Use((context, next) =>
            {
                if (context.Request.Headers.TryGetValue(_xForwardedPathBaseHeader, out var pathBases))
                {
                    context.Request.PathBase = pathBases[0];
                }
                return next();
            });

        return app;
    }

    private static IApplicationBuilder UseSwaggerWithReverseProxySupport(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetService<ILogger>();

        _ = app.UseOpenApi(x =>
            {
                x.CreateDocumentCacheKey = CreateSwaggerDocumentCacheKey;
                x.PostProcess = (document, request) => SwaggerPostProcess(document, request, logger);
            });

        _ = app.UseSwaggerUi(x =>
            {
                if (_settings.OpenApiSettings.Path is not null)
                {
                    x.Path = $"/{_settings.OpenApiSettings.Path.TrimStart('/')}";
                }
                x.TransformToExternalPath = (route, request) =>
                {
                    var pathBase = request.Headers[_xForwardedPathBaseHeader].FirstOrDefault();
                    return string.IsNullOrEmpty(pathBase)
                        ? route
                        : $"{pathBase}{route}";
                };
            });

        return app;
    }

    private static string CreateSwaggerDocumentCacheKey(Microsoft.AspNetCore.Http.HttpRequest request)
    {
        return string.Concat(
            string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault())
                ? request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault()
                : request.Headers["X-Azure-FDID"].FirstOrDefault(),
            request.Headers[_xForwardedPathBaseHeader].FirstOrDefault(),
            request.IsHttps);
    }

    private static void SwaggerPostProcess(OpenApiDocument document, HttpRequest request, ILogger? logger)
    {
        bool isAzureFD = !string.IsNullOrEmpty(request.Headers["X-Azure-FDID"].FirstOrDefault());
        var originalHost = isAzureFD ? string.Empty : request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? request.Headers["X-ORIGINAL-HOST"].FirstOrDefault();
        var pathBase = request.Headers[_xForwardedPathBaseHeader].FirstOrDefault();

        logger?.LogInformation("OriginalHost: {Host}. PathBase: {Path}", originalHost, pathBase);

        if (!string.IsNullOrEmpty(pathBase))
        {
            document.BasePath = pathBase;
        }

        if (!string.IsNullOrEmpty(originalHost))
        {
            document.Host = originalHost;
        }

        // Append build/deployment info to the Swagger Description so it's visible in the UI
        var buildInfo = GetBuildOrDeploymentInfo();
        if (!string.IsNullOrWhiteSpace(buildInfo))
        {
            var info = document.Info ??= new OpenApiInfo();
            var current = info.Description;
            var updated = string.IsNullOrWhiteSpace(current) ? buildInfo : string.Concat(current, "\n\n", buildInfo);
            info.Description = updated;
        }
    }

    private static string? GetBuildOrDeploymentInfo()
    {
        // Prefer a deployment date set at deploy-time (e.g., via App Service App Setting or container env var)
        // Fall back to build date or assembly write time if not provided
        var commit = Environment.GetEnvironmentVariable("APP_BUILD_COMMIT")
            ?? Environment.GetEnvironmentVariable("BUILD_COMMIT")
            ?? Environment.GetEnvironmentVariable("GIT_COMMIT")
            ?? Environment.GetEnvironmentVariable("SOURCE_VERSION");

        var dateRaw = Environment.GetEnvironmentVariable("APP_DEPLOYMENT_DATE")
            ?? Environment.GetEnvironmentVariable("DEPLOYMENT_DATE")
            ?? Environment.GetEnvironmentVariable("BUILD_DATE");

        DateTimeOffset? dto = null;
        if (!string.IsNullOrWhiteSpace(dateRaw)
            && DateTimeOffset.TryParse(dateRaw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AllowWhiteSpaces, out var parsed))
        {
            dto = parsed.ToUniversalTime();
        }

        if (dto is null)
        {
            try
            {
                var asmPath = Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrWhiteSpace(asmPath) && File.Exists(asmPath))
                {
                    dto = File.GetLastWriteTimeUtc(asmPath);
                }
            }
            catch
            {
                // ignore
            }
        }

        if (dto is null && string.IsNullOrWhiteSpace(commit))
        {
            return null;
        }

        var datePart = dto?.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
        return commit is null
            ? $"Deployed: {datePart}"
            : $"Deployed: {datePart ?? "unknown"} | Commit: {commit}";
    }

    private static void RegisteredServicesPage(IApplicationBuilder app, IServiceCollection services)
    {
        _ = app.Map("/services", x => x.Run(async context =>
                                        {
                                            var sb = new StringBuilder();
                                            _ = sb.Append("<h1>Registered Services</h1>")
                                                .Append("<table><thead>")
                                                .Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>")
                                                .Append("</thead><tbody>");
                                            foreach (var svc in services)
                                            {
                                                _ = sb.Append("<tr>")
                                                    .Append("<td>")
                                                    .Append(svc.ServiceType.FullName)
                                                    .Append("</td>")
                                                    .Append("<td>")
                                                    .Append(svc.Lifetime)
                                                    .Append("</td>")
                                                    .Append("<td>")
                                                    .Append(svc.ImplementationType?.FullName)
                                                    .Append("</td>")
                                                    .Append("</tr>");
                                            }
                                            _ = sb.Append("</tbody></table>");
                                            await context.Response.WriteAsync(sb.ToString());
                                        }));
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        _ = services.AddAuthorization(x =>
            {
                x.AddPolicy(AuthPolicies.Basic, policy =>
                    {
                        policy.AuthenticationSchemes.Add(AuthConstants.BasicAuthScheme);
                        _ = policy.RequireAuthenticatedUser();
                    });
                x.AddPolicy(AuthPolicies.ApiKey, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(AuthConstants.ApiKeyAuthScheme);
                        _ = policy.RequireAuthenticatedUser();
                    });
                x.AddPolicy(AuthPolicies.OfficeUser, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                        _ = policy.RequireAuthenticatedUser();
                        if (_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
                        {
                            policy.Requirements.Add(new SessionRequirement());
                        }
                        policy.Requirements.Add(new OfficeUserRequirement());
                    });
                x.AddPolicy(AuthPolicies.Session, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(AuthConstants.SessionAuthScheme);
                        _ = policy.RequireAuthenticatedUser();

                        policy.Requirements.Add(new SessionRequirement());
                    });
                x.AddPolicy(AuthPolicies.TokenOrSession, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, AuthConstants.SessionAuthScheme);
                        _ = policy.RequireAuthenticatedUser();

                        if (_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
                        {
                            policy.Requirements.Add(new SessionRequirement());
                        }

                        policy.Requirements.Add(new OfficeUserRequirement());
                    });
                x.AddPolicy(AuthPolicies.TokenOrSessionOrApiKey, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, AuthConstants.SessionAuthScheme, AuthConstants.ApiKeyAuthScheme);
                        _ = policy.RequireAuthenticatedUser();

                        if (_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
                        {
                            policy.Requirements.Add(new SessionRequirement());
                        }
                        policy.Requirements.Add(new OfficeUserRequirement());
                    });
                x.AddPolicy(AuthPolicies.SuperAdmin, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                        _ = policy.RequireAuthenticatedUser();
                        if (_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
                        {
                            policy.Requirements.Add(new SessionRequirement());
                        }
                        policy.Requirements.Add(new SuperAdminUserRequirement());
                    });
                x.AddPolicy(AuthPolicies.WebHookApiKey, policy =>
                    {
                        _ = policy.AddAuthenticationSchemes(AuthConstants.WebhookApiAuthScheme);
                        _ = policy.RequireAuthenticatedUser();
                    });

            });

        // add authorization handlers
        _ = services.AddAuthHandlers();

        return services;
    }

    private static IServiceCollection AddAuthHandlers(this IServiceCollection services)
    {
        _ = services.AddScoped<IAuthorizationHandler, OfficeUserAuthorizationHandler>();
        _ = services.AddScoped<IAuthorizationHandler, SuperAdminUserRequirementHandler>();
        _ = services.AddScoped<IAuthorizationHandler, SessionRequirementHandler>();
        _ = services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        _ = services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        _ = services.AddSingleton<IAuthorizationMiddlewareResultHandler, SessionAuthorizationResultHandler>();
        _ = services.AddScoped<ValidateBadRequestCommandFilter>();

        return services;
    }

    private static IServiceCollection AddApiClients(this IServiceCollection services)
    {
        _ = services.AddSingleton<IIdentityApiClient, IdentityApiClient>();
        _ = services.AddSingleton<ILendersApiClient, LendersApiClient>();
        _ = services.AddSingleton<IConfigApiClient, ConfigApiClient>();
        _ = services.AddSingleton<IFilesApiClient, FilesApiClient>();
        _ = services.AddSingleton<ILoansApiClient, LoansApiClient>();
        _ = services.AddSingleton<IUrlShortenerApiClient, UrlShortenerApiClient>();
        _ = services.AddSingleton<IMessagingApiClient, MessagingApiClient>();

        return services;
    }
}

#pragma warning restore CA1851 // Possible multiple enumerations of IEnumerable collection
#pragma warning restore CA1724 // Type names should not match namespaces
