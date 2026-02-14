using FluentValidation;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace W2K.Common.Infrastructure.AppInsights;

public class AppInsightsProcessor(ITelemetryProcessor next) : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next = next;

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry request
            && (request.Url.AbsolutePath.StartsWith("/hc", StringComparison.OrdinalIgnoreCase)
                || request.Url.AbsolutePath.StartsWith("/liveness", StringComparison.OrdinalIgnoreCase)
                || request.Url.AbsolutePath.StartsWith("/swagger/", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        if (item is DependencyTelemetry dependency
            && dependency.ResultCode == StatusCodes.Status401Unauthorized.ToString()
            && dependency.Data.EndsWith("hc=true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (item is TraceTelemetry trace
            && trace.Properties.TryGetValue("Uri", out var uri)
            && uri.EndsWith("hc=true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (item is ExceptionTelemetry ex && ex.Exception is ValidationException)
        {
            return;
        }

        _next.Process(item);
    }
}
