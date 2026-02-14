using DFI.Common.Entities;

namespace DFI.Common.Events;

/// <summary>
/// Event used when an entity is deleted.
/// </summary>
public record EntityDeletedDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
{
    public TEntity? Entity { get; private set; }

    public EntityDeletedDomainEvent(TEntity? entity)
    {
        Entity = entity;
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }

    public EntityDeletedDomainEvent(TEntity? entity, int? eventOfficeId)
    {
        Entity = entity;
        SetOffice(eventOfficeId);
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }
}
