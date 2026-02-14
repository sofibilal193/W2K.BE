using W2K.Common.Application.Cacheing;
using W2K.Common.Infrastructure.Cacheing;
using W2K.Common.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeDetailsQueryCachePolicy() : ICachePolicy<GetOfficeDetailsQuery, OfficeDto>
{

    public string AppName => IdentityConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetOfficeDetailsQuery request, OfficeDto response)
    {
        return TimeSpan.FromMinutes(IdentityConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetOfficeDetailsQuery request)
    {
        return $"{CacheConstants.GetOfficeDetailsQueryCachePrefix}:{request.OfficeId}";
    }
}
