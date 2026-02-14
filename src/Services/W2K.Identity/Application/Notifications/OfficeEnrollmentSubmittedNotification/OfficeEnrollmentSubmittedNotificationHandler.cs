using W2K.Common.Application.Messaging;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Settings;
using Microsoft.Extensions.Options;
using W2K.Common.ValueObjects;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class OfficeEnrollmentSubmittedNotificationHandler(IEmailProvider emailProvider, IOptions<ServiceSettings> setting, ICurrentUser currentUser, IMediator mediator) : INotificationHandler<OfficeEnrollmentSubmittedNotification>
{
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly ServiceSettings _setting = setting.Value;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;

    public async Task Handle(OfficeEnrollmentSubmittedNotification notification, CancellationToken cancellationToken)
    {
        var newUserTemplate = _setting.EmailTemplates["NewEnrollmentEmail"];

        var officeAddress = string.Empty;
        if (notification.Address is not null)
        {
            officeAddress = BuildOfficeAddress(notification.Address);
        }

        var emailMessage = new EmailMessage
        {
            From = new EmailAddress
            {
                Name = newUserTemplate.FromName,
                Address = newUserTemplate.FromAddress
            },
            To = new List<EmailAddress>
            {
                new()
                {
                    Address = newUserTemplate.ToAddress ?? string.Empty
                }
            }.AsReadOnly(),
            TemplateId = newUserTemplate.TemplateId,
            TemplateData = new
            {
                EmailSubject = IdentityConstants.NewOfficeEnrollment,
                notification.OfficeId,
                notification.OfficeName,
                OfficeContact = notification.OfficePhone,
                OfficeAddress = officeAddress,
                IsSuperAdminNotification = true,
                IsOfficeApproved = false,
                RecipientName = newUserTemplate.ToName ?? string.Empty,
                PortalUrl = _setting.ClientPortalBaseUrl
            }
        };

        _ = await _emailProvider.SendAsync(new List<EmailMessage> { emailMessage }.AsReadOnly(), cancellationToken);

        await _mediator.Publish(new IdentityEventLogNotification("Email Send to SA", _currentUser.Source, $"Email was send to {newUserTemplate.ToName} from office {notification.OfficeName} ", _currentUser.UserId, notification.OfficeId), cancellationToken);
    }

    private static string BuildOfficeAddress(Address address)
    {
        return $"{address.Address1} {address.Address2}, {address.City} {address.State} {address.ZipCode}".Trim();
    }
}
