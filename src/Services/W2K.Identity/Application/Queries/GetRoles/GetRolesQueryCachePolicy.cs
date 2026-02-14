using W2K.Identity.Application.DTOs;
using W2K.Common.Application.Cacheing;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetRolesQueryCachePolicy : ICachePolicy<GetRolesQuery, IList<RoleDto>>
{
    public string AppName { get; } = IdentityConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetRolesQuery request, IList<RoleDto> response)
    {
        return TimeSpan.FromMinutes(IdentityConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetRolesQuery request)
    {
        // Use the request type name as the cache key
        return $"{nameof(GetRolesQuery)}:{request.OfficeType}";
    }
}
