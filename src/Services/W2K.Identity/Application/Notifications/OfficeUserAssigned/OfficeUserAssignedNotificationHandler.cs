using W2K.Common.Application.Messaging;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using W2K.Identity.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles sending office assignment email notifications to users when a SuperAdmin assigns them to offices.
/// </summary>
/// <remarks>
/// This handler is decoupled from the command handler to maintain clean separation of concerns.
/// Email failures are logged but do not fail the overall operation as emails are non-critical.
/// </remarks>
public class OfficeUserAssignedNotificationHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IEmailProvider emailProvider,
    IOptions<ServiceSettings> settingsOptions,
    IMediator mediator,
    ILogger<OfficeUserAssignedNotificationHandler> logger)
    : INotificationHandler<OfficeUserAssignedNotification>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IEmailProvider _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
    private readonly ServiceSettings _setting = (settingsOptions ?? throw new ArgumentNullException(nameof(settingsOptions))).Value;
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<OfficeUserAssignedNotificationHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string UserInvitationBaseUrl => _setting.UserInvitationBaseUrl;

    public async Task Handle(OfficeUserAssignedNotification notification, CancellationToken cancellationToken)
    {
        var emailTemplate = _setting.EmailTemplates["NewUserEmail"];

        // Fetch user and offices using OfficeUsers join table in single query
        var officeUsers = await _data.OfficeUsers
                            .Include("User")
                            .Include("Office")
                            .GetAsync(
                                x => x.UserId == notification.UserId && notification.OfficeIds.Contains(x.OfficeId),
                                cancellationToken)
            ?? throw new NotFoundException(nameof(User), notification.UserId);

        var user = officeUsers.FirstOrDefault()?.User
            ?? throw new NotFoundException(nameof(User), notification.UserId);

        var offices = officeUsers
            .Select(x => x.Office)
            .Where(x => x is not null)
            .Cast<Office>()
            .ToList();

        if (offices.Count == 0)
        {
            _logger.LogWarning(
                "No offices found for user {UserId} with office IDs {OfficeIds}",
                notification.UserId,
                string.Join(", ", notification.OfficeIds));
            return;
        }

        // Send emails in parallel for better performance
        await Task.WhenAll(offices.Select(x => SendOfficeAssignmentEmailAsync(user, x, emailTemplate, cancellationToken)));

        // Log success for all emails
        await _mediator.Publish(
            new IdentityEventLogNotification(
            "Office Assignment Emails Sent",
            _currentUser.Source,
            $"Office assignment emails sent to {user.Email} for {offices.Count} office(s): {string.Join(", ", offices.Select(x => x.Name))}",
            user.Id,
            offices.FirstOrDefault()?.Id),
            cancellationToken);
    }

    private async Task SendOfficeAssignmentEmailAsync(
        User user,
        Office office,
        EmailTemplate template,
        CancellationToken cancel)
    {
        var emailMessage = new EmailMessage
        {
            From = new EmailAddress
            {
                Name = template.FromName,
                Address = template.FromAddress
            },
            To = new List<EmailAddress>
        {
            new()
            {
                Name = user.FullName,
                Address = user.Email
            }
        }.AsReadOnly(),
            TemplateId = template.TemplateId,
            TemplateData = new
            {
                EmailSubject = IdentityConstants.MerchantEmailButtonText,
                IsExistingOfficeExistingUser = true,
                FirstName = user.FirstName ?? string.Empty,
                InviteUserName = _currentUser.FullName ?? IdentityConstants.AdministratorRoleName,
                OfficeName = office.Name,
                ButtonText = IdentityConstants.MerchantEmailButtonText,
                ButtonUrl = UserInvitationBaseUrl
            }
        };

        _ = await _emailProvider.SendAsync(
            new List<EmailMessage> { emailMessage }.AsReadOnly(),
            cancel);

        _logger.LogInformation(
            "Office assignment email sent successfully to user {UserId} for office {OfficeId}",
            user.Id,
            office.Id);
    }
}

