using System.Security.Claims;
using W2K.Common.Application.Auth;
using W2K.Common.Application.Identity;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace W2K.Common.Infrastructure.AppInsights;

public class AppInsightsInitializer(IHttpContextAccessor httpContextAccessor) : ITelemetryInitializer
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public void Initialize(ITelemetry telemetry)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        SetAuthenticatedUser(httpContext, telemetry);
        SetSessionId(httpContext, telemetry);
        SetCustomProperties(httpContext, telemetry as ISupportProperties);
    }

    private static void SetAuthenticatedUser(HttpContext httpContext, ITelemetry telemetry)
    {
        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            telemetry.Context.User.AuthenticatedUserId = httpContext.User.FindFirst(ClaimTypes.Sid)?.Value;
        }
    }

    private static void SetSessionId(HttpContext httpContext, ITelemetry telemetry)
    {
        var sessionId = GetHeaderValue(httpContext, AuthConstants.AppInsightsSessionIdHeaderName);
        if (!string.IsNullOrEmpty(sessionId))
        {
            telemetry.Context.Session.Id = sessionId;
        }
    }

    private static void SetCustomProperties(HttpContext httpContext, ISupportProperties? supportProperties)
    {
        if (supportProperties is null)
        {
            return;
        }

        AddHeaderProperty(httpContext, supportProperties, AuthConstants.SessionIdHeaderName);
        AddHeaderProperty(httpContext, supportProperties, AuthConstants.FingerPrintHeaderName);
        AddClientIpProperty(httpContext, supportProperties);
    }

    private static void AddHeaderProperty(HttpContext httpContext, ISupportProperties supportProperties, string headerName)
    {
        var headerValue = GetHeaderValue(httpContext, headerName);
        if (!string.IsNullOrEmpty(headerValue))
        {
            supportProperties.Properties[headerName] = headerValue;
        }
    }

    private static void AddClientIpProperty(HttpContext httpContext, ISupportProperties supportProperties)
    {
        var clientIp = httpContext.GetRequestIp();
        if (!string.IsNullOrEmpty(clientIp))
        {
            supportProperties.Properties["ClientIp"] = clientIp;
        }
    }

    private static string? GetHeaderValue(HttpContext httpContext, string headerName)
    {
        if (httpContext.Request?.Headers?.TryGetValue(headerName, out var value) == true && !string.IsNullOrEmpty(value))
        {
            return value.ToString();
        }

        return null;
    }
}
