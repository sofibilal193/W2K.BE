using W2K.Common.Application.Cacheing;
using W2K.Common.Events;
using W2K.Common.Infrastructure.Cacheing;
using W2K.Identity.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Events;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class OfficeUserUpsertedDomainEventHandler(ICache cache)
    : INotificationHandler<EntityCreatedDomainEvent<OfficeUser>>, INotificationHandler<EntityUpdatedDomainEvent<OfficeUser>>, INotificationHandler<EntityDeletedDomainEvent<OfficeUser>>
{
    private readonly ICache _cache = cache;

    public async Task Handle(EntityCreatedDomainEvent<OfficeUser> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.UserId, cancellationToken);
    }

    public async Task Handle(EntityUpdatedDomainEvent<OfficeUser> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.UserId, cancellationToken);
    }

    public async Task Handle(EntityDeletedDomainEvent<OfficeUser> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.UserId, cancellationToken);
    }

    private async Task RemoveUserOfficeListCacheAsync(int? userId, CancellationToken cancel)
    {
        await _cache.RemoveAsync(
            IdentityConstants.ApplicationName,
            $"{CacheConstants.GetUserOfficeListQueryCachePrefix}:{userId}",
            cancel);
    }
}
