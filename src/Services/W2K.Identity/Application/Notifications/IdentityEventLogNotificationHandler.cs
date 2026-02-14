using Microsoft.Extensions.Logging;

namespace W2K.Identity.Application.Notifications;

public class IdentityEventLogNotificationHandler(ILogger<IdentityEventLogNotificationHandler> logger)
    : INotificationHandler<IdentityEventLogNotification>
{
    private readonly ILogger<IdentityEventLogNotificationHandler> _logger = logger;

    public Task Handle(IdentityEventLogNotification notification, CancellationToken cancellationToken)
    {
        // Log the event for audit purposes
        _logger.LogInformation(
            "Identity Event Log: Type={EventType}, Source={Source}, Description={Description}, UserId={UserId}, OfficeId={OfficeId}",
            notification.EventType,
            notification.Source,
            notification.Description,
            notification.UserId,
            notification.OfficeId);
        return Task.CompletedTask;
    }
}
