using Microsoft.EntityFrameworkCore;
using MediatR;
using DFI.Common.Identity;
using DFI.Common.Entities;

namespace DFI.Common.Persistence.Extensions;

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
