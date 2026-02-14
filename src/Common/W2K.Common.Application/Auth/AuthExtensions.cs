using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace W2K.Common.Application.Auth;

public static class AuthExtensions
{
    public static AuthenticationBuilder AddJwtAuth(this AuthenticationBuilder builder, ILogger logger)
    {
        _ = builder.Services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();
        _ = builder.AddJwtBearer(
            x =>
            {
                // add event to get access token from query string for websocket connections
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        logger.LogTrace("OnMessageReceived: Path: {Path}", context.Request.Path.Value);
                        if (context.Request.Path.Value?.StartsWith("/hubs/", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                logger.LogTrace("Setting access token from URL.");
                                context.Token = accessToken;
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        return builder;
    }

    // https://docs.microsoft.com/en-us/azure/active-directory-b2c/enable-authentication-web-api?tabs=csharpclient
    // https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-web-api-aspnet-core-protect-api
    public static AuthenticationBuilder AddApiAuth(this AuthenticationBuilder builder, IConfiguration config, ILogger logger)
    {
        _ = builder.Services.AddSingleton<IConfigureOptions<MicrosoftIdentityOptions>, MicrosoftIdentityOptionsSetup>();
        _ = builder.AddMicrosoftIdentityWebApi(config);

        _ = builder.Services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            x =>
            {
                x.TokenValidationParameters ??= new();
                x.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
                AttachJwtDiagnostics(x, logger);
            });

        return builder;
    }

    private static void AttachJwtDiagnostics(JwtBearerOptions options, ILogger logger)
    {
        var original = options.Events ?? new JwtBearerEvents();

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async x =>
            {
                try { LogAuthFailureTokenDiagnostics(x); } catch (Exception ex) { logger.LogError(ex, "Failed to log auth diagnostics"); }
                logger.LogError(x.Exception, "Authentication failed: {Message}", x.Exception.Message);
                if (original.OnAuthenticationFailed is not null)
                {
                    await original.OnAuthenticationFailed(x);
                }
            },
            OnTokenValidated = async x =>
            {
                logger.LogInformation("Token validated for: {Subject}", x.Principal?.Identity?.Name);
                if (original.OnTokenValidated is not null)
                {
                    await original.OnTokenValidated(x);
                }
            },
            OnMessageReceived = async x =>
            {
                logger.LogInformation("OnMessageReceived: Path: {Path}", x.Request.Path.Value);
                if (original.OnMessageReceived is not null)
                {
                    await original.OnMessageReceived(x);
                }
            },
            OnChallenge = async x =>
            {
                logger.LogWarning("Authentication challenge: {Error} - {Description}", x.Error, x.ErrorDescription);
                if (original.OnChallenge is not null)
                {
                    await original.OnChallenge(x);
                }
            }
        };
    }

    private static void LogAuthFailureTokenDiagnostics(AuthenticationFailedContext context)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return;
        }

        var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader["Bearer ".Length..]
            : authHeader;

        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return;
        }

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var exp = jwt.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
        var nbf = jwt.Claims.FirstOrDefault(x => x.Type == "nbf")?.Value;
        var jti = jwt.Claims.FirstOrDefault(x => x.Type == "jti")?.Value;
        var kid = jwt.Header.TryGetValue("kid", out var v) ? v?.ToString() : null;

        context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("Auth.Diagnostics")
            .LogWarning(
                "Auth failed. kid={Kid}, jti={Jti}, exp={Exp} ({ExpUtc}), nbf={Nbf} ({NbfUtc})",
                kid,
                jti,
                exp,
                long.TryParse(exp, out var expSec) ? DateTimeOffset.FromUnixTimeSeconds(expSec).UtcDateTime : null,
                nbf,
                long.TryParse(nbf, out var nbfSec) ? DateTimeOffset.FromUnixTimeSeconds(nbfSec).UtcDateTime : null);
    }
}
