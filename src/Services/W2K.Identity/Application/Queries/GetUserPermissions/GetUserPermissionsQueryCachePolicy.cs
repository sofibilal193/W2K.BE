using W2K.Common.Application.Cacheing;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserPermissionsQueryCachePolicy : ICachePolicy<GetUserPermissionsQuery, IList<string>>
{
    public string AppName { get; } = IdentityConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetUserPermissionsQuery request, IList<string> response)
    {
        return TimeSpan.FromMinutes(IdentityConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetUserPermissionsQuery request)
    {
        // Use the request type name as the cache key
        return $"{nameof(GetUserPermissionsQuery)}.{request.UserId}.{request.OfficeId}";
    }
}
