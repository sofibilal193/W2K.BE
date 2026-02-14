using MediatR;

namespace DFI.Common.Events;

/// <summary>
/// Base event all domain events should inherit from.
/// </summary>
public abstract record DomainEvent
     : INotification
{
    /// <summary>
    /// Id of User who triggered the event
    /// </summary>
    public int? EventUserId { get; private set; }

    /// <summary>
    /// Name of user who triggered the event
    /// </summary>
    public string? EventUserName { get; private set; }

    /// <summary>
    /// Id of Office who logged-in user triggered the event
    /// </summary>
    public int? EventOfficeId { get; private set; }

    /// <summary>
    /// Client Ip address of user who triggered the event
    /// </summary>
    public string? EventSource { get; private set; }

    protected DomainEvent() { }

    protected DomainEvent(int? eventUserid, string? eventUserName, string? eventSource = null, int? eventOfficeId = null)
    {
        EventUserId = eventUserid;
        EventUserName = eventUserName;
        EventSource = eventSource;
        EventOfficeId = eventOfficeId;
    }

    public void SetOffice(int? eventOfficeId)
    {
        EventOfficeId = eventOfficeId;
    }

    public void SetUserSource(int? eventUserid, string? eventUserName, string? eventSource = null)
    {
        if (eventUserid is not null)
        {
            EventUserId = eventUserid;
        }

        if (eventUserName is not null)
        {
            EventUserName = eventUserName;
        }

        if (eventSource is not null)
        {
            EventSource = eventSource;
        }
    }
}
