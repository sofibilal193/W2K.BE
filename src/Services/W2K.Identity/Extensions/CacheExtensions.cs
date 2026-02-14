using System.Collections.ObjectModel;
using W2K.Common.Application.Cacheing;
using W2K.Identity.Entities;

namespace W2K.Identity.Extensions;

public static class CacheExtensions
{
    private const string _superAdminOfficeIdsKey = "SuperAdminOfficeIds";

    private const string _officeGroupsKey = "OfficeGroups";

    #region SuperAdminOfficeIds

    public static async Task<List<int>?> GetSuperAdminOfficeIdsAsync(this ICache cache, CancellationToken cancel)
    {
        return await cache.GetAsync<List<int>>(IdentityConstants.ApplicationName, _superAdminOfficeIdsKey, cancel);
    }

    public static async Task SetSuperAdminOfficeIdsAsync(this ICache cache, ReadOnlyCollection<int> officeIds, CancellationToken cancel)
    {
        await cache.SetAsync(IdentityConstants.ApplicationName, _superAdminOfficeIdsKey, officeIds, cancel);
    }

    public static async Task ClearSuperAdminOfficeIdsAsync(this ICache cache, CancellationToken cancel)
    {
        await cache.RemoveAsync(IdentityConstants.ApplicationName, _superAdminOfficeIdsKey, cancel);
    }

    #endregion

    #region OfficeGroups

    public static async Task<List<OfficeGroup>?> GetOfficeGroupsAsync(this ICache cache, CancellationToken cancel)
    {
        return await cache.GetAsync<List<OfficeGroup>>(IdentityConstants.ApplicationName, _officeGroupsKey, cancel);
    }

    public static async Task SetOfficeGroupsAsync(this ICache cache, ReadOnlyCollection<OfficeGroup> groups, CancellationToken cancel)
    {
        await cache.SetAsync(IdentityConstants.ApplicationName, _officeGroupsKey, groups, cancel);
    }

    public static async Task ClearOfficeGroupsAsync(this ICache cache, CancellationToken cancel)
    {
        await cache.RemoveAsync(IdentityConstants.ApplicationName, _officeGroupsKey, cancel);
    }

    #endregion
}
