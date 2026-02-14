using W2K.Common.Application.Auth;
using W2K.Common.Application.Settings;
using W2K.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Application.DependencyInjection;

public static class CorsServices
{
    public static IServiceCollection AddCorsPolicies(
        this IServiceCollection services,
        AppSettings settings,
        ILogger logger)
    {
        // Expected format for AllowedCorsOrigins: "https://app.example.com;https://admin.example.com"
        var raw = settings.AllowedCorsOrigins;
        if (string.IsNullOrWhiteSpace(raw))
        {
            logger.LogWarning("No AllowedCorsOrigins configured. CORS policy will not be registered.");
            return services;
        }

        var origins = raw.SplitToList(';')!
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Length == 0)
        {
            logger.LogWarning("AllowedCorsOrigins was provided but no valid origins were parsed from: {Val}", raw);
            return services;
        }

        logger.LogInformation("Registering CORS for {Count} origin(s).", origins.Length);

        // Explicitly list allowed headers we expect from the SPA / clients.
        // If new custom headers are introduced, append here rather than using AllowAnyHeader.
        var allowedRequestHeaders = new[]
        {
            "Content-Type",
            "Accept",
            "Authorization", // bearer tokens if used
            "X-Requested-With",
            AuthConstants.AppInsightsSessionIdHeaderName,
            AuthConstants.SessionIdHeaderName,
            AuthConstants.FingerPrintHeaderName,
        };

        // Expose only custom headers needed by the client (previously request-context only)
        var exposedHeaders = new[]
        {
            "request-context",
            AuthConstants.SessionIdHeaderName,
            AuthConstants.FingerPrintHeaderName
        };

        _ = services.AddCors(x =>
            {
                x.AddDefaultPolicy(p =>
                    {
                        _ = p.WithOrigins(origins)
                            .WithHeaders(allowedRequestHeaders)
                            .WithMethods(AuthConstants.AllowedCorsMethods)
                            .WithExposedHeaders(exposedHeaders)
                            .AllowCredentials();
                    });
            });

        return services;
    }
}
