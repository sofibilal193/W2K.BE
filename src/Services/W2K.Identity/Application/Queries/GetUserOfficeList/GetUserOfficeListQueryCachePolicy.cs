using W2K.Common.Application.Cacheing;
using W2K.Common.Identity;
using W2K.Common.Infrastructure.Cacheing;
using W2K.Identity.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserOfficeListQueryCachePolicy(ICurrentUser currentUser) : ICachePolicy<GetUserOfficeListQuery, IList<UserOfficeListItemDto>>
{
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public string AppName => IdentityConstants.ApplicationName;

    public TimeSpan? SlidingExpiration(GetUserOfficeListQuery request, IList<UserOfficeListItemDto> response)
    {
        return TimeSpan.FromMinutes(IdentityConstants.DefaultCacheExpirationMinutes);
    }

    public string GetCacheKey(GetUserOfficeListQuery request)
    {
        return $"{CacheConstants.GetUserOfficeListQueryCachePrefix}:{_currentUser.UserId}";
    }
}
