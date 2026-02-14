using W2K.Common.Events;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Repositories;

namespace W2K.Identity.Application.Events;

public class LenderOfficeUpdatedDomainEventHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator) : INotificationHandler<LenderOfficeUpdatedDomainEvent>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _currentUser = currentUser;


    public async Task Handle(LenderOfficeUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.GetAsync(notification.OfficeId, cancellationToken);
        if (office is null)
        {
            return;
        }

        office.SetApproved(notification.IsApproved);

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await _mediator.Publish(
            new IdentityEventLogNotification(
            "lender approval status updated",
            _currentUser.Source,
            $"Office ID: {office.Id}, Approval Status: {notification.IsApproved}.",
            _currentUser.UserId,
            office.Id),
            cancellationToken);
    }
}
