using W2K.Common.Application;
using W2K.Common.Application.Cacheing;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetLendersQueryCachePolicy : ICachePolicy<GetLendersQuery, List<LenderDto>>
{
    public string AppName { get; } = IdentityConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetLendersQuery request, List<LenderDto> response)
    {
        return TimeSpan.FromMinutes(IdentityConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetLendersQuery request)
    {
        // Use the request type name as the cache key
        return nameof(GetLendersQuery);
    }
}
