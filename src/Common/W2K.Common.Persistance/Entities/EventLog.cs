#pragma warning disable CA1724 // Type names should not match namespaces
using W2K.Common.Events;

namespace W2K.Common.Persistence.Entities;

public class EventLog
{
    /// <summary>
    /// Database Id of record.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Id of User who triggered the event.
    /// </summary>
    public int? UserId { get; }

    /// <summary>
    /// Id of Office who event is related to.
    /// </summary>
    public int? OfficeId { get; }

    /// <summary>
    /// Id of record who event is related to e.g. LoanApp, Contract etc.
    /// </summary>
    public int? RecordId { get; }

    /// <summary>
    /// Type of event e.g. Created, Updated, Deleted.
    /// </summary>
    public string? Type { get; }

    /// <summary>
    /// Description of event.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Date event occurred in UTC time.
    /// </summary>
    public DateTime DateTimeUtc { get; }

    /// <summary>
    /// User IP address or source of event.
    /// </summary>
    public string? Source { get; }

    /// <summary>
    /// Database timestamp of record.
    /// </summary>
    public byte[]? Timestamp { get; }

    #region Constructors

    public EventLog()
    {
        // Default constructor for EF Core
    }

    public EventLog(
        int? userId,
        int? officeId,
        int? recordId,
        string type,
        string? description,
        DateTime dateTimeUtc,
        string? source)
    {
        UserId = userId;
        OfficeId = officeId;
        RecordId = recordId;
        Type = type;
        Description = description;
        DateTimeUtc = dateTimeUtc;
        Source = source;
    }

    public EventLog(
        int? userId,
        int? officeId,
        int? recordId,
        string type,
        string? description,
        string? source)
    {
        UserId = userId;
        OfficeId = officeId;
        RecordId = recordId;
        Type = type;
        Description = description;
        DateTimeUtc = DateTime.UtcNow;
        Source = source;
    }

    public EventLog(EventLogNotification @event) : this(
        @event.UserId,
        @event.OfficeId,
        @event.RecordId,
        @event.EventType,
        @event.Description,
        @event.DateTimeUtc,
        @event.Source)
    {
    }

    #endregion
}
#pragma warning restore CA1724 // Type names should not match namespaces
