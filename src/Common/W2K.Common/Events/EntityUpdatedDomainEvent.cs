using W2K.Common.Entities;

namespace W2K.Common.Events;

/// <summary>
/// Event used when an entity is updated.
/// </summary>
public record EntityUpdatedDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
{
    public TEntity? Entity { get; private set; }

    public EntityUpdatedDomainEvent(TEntity? entity)
    {
        Entity = entity;
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }

    public EntityUpdatedDomainEvent(TEntity? entity, int? eventOfficeId)
    {
        Entity = entity;
        SetOffice(eventOfficeId);
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }
}
