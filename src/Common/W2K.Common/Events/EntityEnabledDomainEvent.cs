using W2K.Common.Entities;

namespace W2K.Common.Events;

/// <summary>
/// Event used when an entity is enabled.
/// </summary>
public record EntityEnabledDomainEvent<TEntity> : DomainEvent where TEntity : BaseEntity
{
    public TEntity? Entity { get; private set; }

    public EntityEnabledDomainEvent(TEntity? entity)
    {
        Entity = entity;
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }

    public EntityEnabledDomainEvent(TEntity? entity, int? eventOfficeId)
    {
        Entity = entity;
        SetOffice(eventOfficeId);
        SetUserSource(entity?.ModifyUserId, entity?.ModifyUserName, entity?.ModifySource);
    }
}
