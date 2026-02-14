using Microsoft.Extensions.Logging;

namespace W2K.Files.Application.Notifications;

public class FilesEventLogNotificationHandler(ILogger<FilesEventLogNotificationHandler> logger)
    : INotificationHandler<FilesEventLogNotification>
{
    private readonly ILogger<FilesEventLogNotificationHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public Task Handle(FilesEventLogNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        // Log the event for audit purposes
        _logger.LogInformation(
            "Files Event Log: Type={EventType}, Source={Source}, Description={Description}, UserId={UserId}, OfficeId={OfficeId}, FileId={FileId}",
            notification.EventType,
            notification.Source,
            notification.Description,
            notification.UserId,
            notification.OfficeId,
            notification.RecordId);

        return Task.CompletedTask;
    }
}
