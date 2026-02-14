#pragma warning disable CA1819 // Properties should not return arrays

using System.Text.Json.Serialization;
using DFI.Common.Events;

namespace DFI.Common.Entities;

public abstract class BaseEntity : BaseEntityLogProps
{
    private List<DomainEvent>? _domainEvents;

    /// <summary>
    /// Domain events to be published.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyCollection<DomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();

    /// <summary>
    /// Database Id of record.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Database timestamp of record.
    /// </summary>
    public byte[]? Timestamp { get; }

    /// <summary>
    /// Flag to indicate if record has been disabled.
    /// </summary>
    public bool IsDisabled { get; private set; }

    #region Constructors

    protected BaseEntity()
    {
    }

    #endregion

    #region Public Methods

    public virtual void Disable()
    {
        IsDisabled = true;
    }

    public virtual void Enable()
    {
        IsDisabled = false;
    }

    public void AddDomainEvent(DomainEvent eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(DomainEvent eventItem)
    {
        _ = (_domainEvents?.Remove(eventItem));
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    #endregion
}

#pragma warning restore CA1819 // Properties should not return arrays
