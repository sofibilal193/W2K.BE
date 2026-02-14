using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement.FeatureFilters;

namespace W2K.Common.Infrastructure.AppConfig;

public class OfficeTargetingContextAccessor(IHttpContextAccessor httpContextAccessor) : ITargetingContextAccessor
{
    private const string _targetingContextLookup = "OfficeTargetingContextAccessor.TargetingContext";
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public ValueTask<TargetingContext> GetContextAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            if (httpContext.Items.TryGetValue(_targetingContextLookup, out object? value) && value is not null)
            {
                return new ValueTask<TargetingContext>((TargetingContext)value);
            }

            var routeValues = httpContext.Request.RouteValues;
            if (routeValues.TryGetValue("officeId", out var officeId))
            {
                var id = officeId?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var targetingContext = new TargetingContext
                    {
                        Groups =
                        [
                            string.Concat("OfficeId:", id)
                        ]
                    };
                    return new ValueTask<TargetingContext>(targetingContext);
                }
            }
        }

        return new ValueTask<TargetingContext>(new TargetingContext());
    }
}
