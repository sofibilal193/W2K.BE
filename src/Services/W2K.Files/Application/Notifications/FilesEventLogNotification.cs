using W2K.Common.Events;

namespace W2K.Files.Application.Notifications;

public record FilesEventLogNotification : EventLogNotification
{
    public FilesEventLogNotification(
        string eventType,
        string? source,
        string description,
        int? userId,
        int? officeId,
        int? fileId)
            : base(eventType, DateTime.UtcNow, source, description, userId, officeId, fileId)
    {
    }
}
