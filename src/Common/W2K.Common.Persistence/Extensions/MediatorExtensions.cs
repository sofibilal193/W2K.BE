using Microsoft.EntityFrameworkCore;
using MediatR;
using W2K.Common.Identity;
using W2K.Common.Entities;

namespace W2K.Common.Persistence.Extensions;

internal static class MediatorExtensions
{
    internal static async Task DispatchDomainEventsAsync(
         this IPublisher mediator,
         DbContext ctx,
         ICurrentUser user,
         CancellationToken cancel = default)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents?.Count > 0);

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents!)
            .ToList();

        foreach (var entity in domainEntities.ToList())
        {
            entity.Entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            domainEvent.SetUserSource(user.UserId, user.FullName, user.Source);
            await mediator.Publish(domainEvent, cancel);
        }
    }
}
