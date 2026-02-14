using W2K.Common.Application.Cacheing;
using W2K.Common.Events;
using W2K.Common.Infrastructure.Cacheing;
using W2K.Identity.Entities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Events;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UserUpsertedDomainEventHandler(ICache cache)
    : INotificationHandler<EntityCreatedDomainEvent<User>>, INotificationHandler<EntityUpdatedDomainEvent<User>>, INotificationHandler<EntityDeletedDomainEvent<User>>
{
    private readonly ICache _cache = cache;

    public async Task Handle(EntityCreatedDomainEvent<User> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.Id, cancellationToken);
    }

    public async Task Handle(EntityUpdatedDomainEvent<User> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.Id, cancellationToken);
    }

    public async Task Handle(EntityDeletedDomainEvent<User> notification, CancellationToken cancellationToken)
    {
        await RemoveUserOfficeListCacheAsync(notification.Entity?.Id, cancellationToken);
    }

    private async Task RemoveUserOfficeListCacheAsync(int? userId, CancellationToken cancel)
    {
        await _cache.RemoveAsync(
            IdentityConstants.ApplicationName,
            $"{CacheConstants.GetUserOfficeListQueryCachePrefix}:{userId}",
            cancel);
    }
}
