using W2K.Config.Application.DTOs;
using W2K.Common.Application.Cacheing;
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Config.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetExternalConfigsQueryCachePolicy : ICachePolicy<GetExternalConfigsQuery, IList<ConfigDto>>
{
    public string AppName { get; } = ConfigConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetExternalConfigsQuery request, IList<ConfigDto> response)
    {
        return TimeSpan.FromMinutes(ConfigConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetExternalConfigsQuery request)
    {
        // Use the request type name as the cache key
        return $"{nameof(GetExternalConfigsQuery)}";
    }
}
