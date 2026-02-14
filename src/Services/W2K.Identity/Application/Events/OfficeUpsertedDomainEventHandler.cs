using W2K.Common.Application.Cacheing;
using W2K.Common.Events;
using W2K.Common.Infrastructure.Cacheing;
using W2K.Identity.Application.Queries;
using W2K.Identity.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Events;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles the EntityUpdatedDomainEvent for Office entities.
/// Invalidates cache entries for the updated office and user office lists.
/// </summary>
public class OfficeUpsertedDomainEventHandler(
    ICache cache) : INotificationHandler<EntityCreatedDomainEvent<Office>>, INotificationHandler<EntityUpdatedDomainEvent<Office>>
{
    private readonly ICache _cache = cache;

    public async Task Handle(EntityCreatedDomainEvent<Office> notification, CancellationToken cancellationToken)
    {
        var office = notification.Entity;

        if (office is null)
        {
            return;
        }

        await ClearOfficeCacheAsync(office.Id, cancellationToken);
    }

    public async Task Handle(EntityUpdatedDomainEvent<Office> notification, CancellationToken cancellationToken)
    {
        var office = notification.Entity;

        if (office is null)
        {
            return;
        }

        await ClearOfficeCacheAsync(office.Id, cancellationToken);
    }

    private async Task ClearOfficeCacheAsync(int officeId, CancellationToken cancel)
    {
        // Clear the specific office details cache
        var query = new GetOfficeDetailsQuery(officeId);
        var policy = new GetOfficeDetailsQueryCachePolicy();
        var cacheKey = policy.GetCacheKey(query);

        await _cache.RemoveAsync(
            policy.AppName,
            cacheKey,
            cancel);

        // Clear the user office list caches for all users
        var userOfficeCachePattern = $"{CacheConstants.GetUserOfficeListQueryCachePrefix}:*";

        await _cache.RemoveAllAsync(
            IdentityConstants.ApplicationName,
            userOfficeCachePattern,
            cancel);
    }
}
