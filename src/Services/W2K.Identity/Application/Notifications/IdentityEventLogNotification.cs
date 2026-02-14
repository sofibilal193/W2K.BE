using W2K.Common.Events;

namespace W2K.Identity.Application.Notifications;

public record IdentityEventLogNotification
    : EventLogNotification
{
    public IdentityEventLogNotification(
        string eventType,
        string? source,
        string description,
        int? userId,
        int? officeId)
        : base(eventType, DateTime.UtcNow, source, description, userId, officeId, null)
    {
    }
}
