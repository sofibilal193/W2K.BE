using W2K.Common.Application.AzureAd;
using W2K.Common.Application.Messaging;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using W2K.Identity.Settings;
using Microsoft.Extensions.Options;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IEmailProvider emailProvider,
    IOptions<ServiceSettings> settingsOptions,
    ISmsProvider smsProvider,
    IAzureADProvider azureDbProvider)
    : IRequestHandler<UpsertOfficeUserCommand, int?>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IEmailProvider _emailProvider = emailProvider;
    private readonly ServiceSettings _settingsOptions = settingsOptions.Value;
    private readonly ISmsProvider _smsProvider = smsProvider;

    private readonly IAzureADProvider _azureDbProvider = azureDbProvider;

    private string InviteBaseUrl => _settingsOptions.UserInvitationBaseUrl;

    public async Task<int?> Handle(UpsertOfficeUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId is null)
        {
            return await AddOfficeUserAsync(command, cancellationToken);
        }
        else
        {
            await UpdateOfficeUserAsync(command, cancellationToken);
            return command.UserId;
        }
    }

    private async Task<int> AddOfficeUserAsync(UpsertOfficeUserCommand command, CancellationToken cancel)
    {
        // call provider to add new user to Azure AD B2C
        var azureAdUser = new AzureADUser
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            MobilePhone = command.MobilePhone,
            JobTitle = command.Title,
            LastLoginDateTime = DateTime.UtcNow
        };

        var response = await _azureDbProvider.AddUserAsync(azureAdUser, cancel);

        if (response.Status == AzureAdResponseStatus.Success)
        {
            var user = new User(
                response.User.Id,
                command.FirstName,
                command.LastName,
                command.Email,
                command.MobilePhone,
                null // lastLoginIpAddress
            );

            _data.Users.Add(user);

            Office? office = null;

            if (command.OfficeId.HasValue)
            {
                office = await _data.Offices.FirstOrDefaultAsync(x => x.Id == command.OfficeId.Value, cancel)
                    ?? throw new NotFoundException(nameof(Office), command.OfficeId);

                user.UpsertOffice(command.OfficeId.Value, command.RoleId, command.Title, command.IsDefaultOffice);
            }

            _ = await _data.SaveEntitiesAsync(cancel);

            await SendEmailAsync(user, InviteBaseUrl, true, command.OfficeId.HasValue, office?.Id, office?.Name, cancel);

            await SendTemporaryPasswordSmsAsync(command.MobilePhone, response.TemporaryPassword, cancel);

            // Conditional notification
            string action;
            string message;

            if (command.OfficeId.HasValue)
            {
                action = "User Created";
                message = $"User: {user.FirstName} {user.LastName} ({user.Email}) added to Office: {office?.Name} (OfficeId: {office?.Id}).";
            }
            else
            {
                action = "User Invited";
                message = $"User: {user.FirstName} {user.LastName} ({user.Email}) invited to enroll.";
            }

            await _mediator.Publish(
                new IdentityEventLogNotification(
                action,
                _currentUser.Source,
                message,
                _currentUser.UserId,
                user.Id),
                cancel);

            return user.Id;
        }
        else
        {
            throw new DomainException($"Failed to create user in Azure AD for email: {command.Email}. Status: {response.Status}");
        }
    }

    private async Task UpdateOfficeUserAsync(UpsertOfficeUserCommand command, CancellationToken cancel)
    {
        var user = await _data.Users.Include(x => x.Offices).FirstOrDefaultAsync(x => x.Id == command.UserId, cancel)
            ?? throw new NotFoundException(nameof(User), command.UserId!.Value);

        Office? office = null;

        // Update office association if OfficeId is provided
        if (command.OfficeId.HasValue)
        {
            office = await _data.Offices.FirstOrDefaultAsync(x => x.Id == command.OfficeId.Value, cancel)
                ?? throw new NotFoundException(nameof(Office), command.OfficeId);

            user.UpsertOffice(command.OfficeId.Value, command.RoleId, command.Title, command.IsDefaultOffice);
        }

        // Update user details in Azure AD if authorized and changes detected
        await UpdateUserInAzureAdIfNeededAsync(user, command, cancel);

        _ = await _data.SaveEntitiesAsync(cancel);

        // Send notification email if requested
        if (command.SendUserNotification)
        {
            await SendEmailAsync(
                user,
                InviteBaseUrl,
                isNewUser: false,
                command.OfficeId.HasValue,
                office?.Id,
                office?.Name,
                cancel
            );
        }

        // Publish event log notification
        await PublishUpdateEventNotificationAsync(user, office, command.OfficeId.HasValue, cancel);
    }

    private async Task UpdateUserInAzureAdIfNeededAsync(User user, UpsertOfficeUserCommand command, CancellationToken cancel)
    {
        // Only SuperAdmin can update user details in Azure AD
        if (_currentUser.OfficeType != OfficeType.SuperAdmin)
        {
            return;
        }

        // Check if user has valid Azure AD identifiers
        if (string.IsNullOrEmpty(user.ProviderId) || string.IsNullOrEmpty(user.Email))
        {
            return;
        }

        var hasMobilePhoneChanged = HasMobilePhoneChanged(command.MobilePhone, user.MobilePhone);
        var hasNameChanged = HasNameChanged(command.FirstName, command.LastName, user.FirstName, user.LastName);

        // No changes detected
        if (!hasMobilePhoneChanged && !hasNameChanged)
        {
            return;
        }

        var azureAdUser = new AzureADUser
        {
            Id = user.ProviderId,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = user.Email,
            MobilePhone = command.MobilePhone,
            JobTitle = command.Title
        };

        var response = await _azureDbProvider.UpdateUserAsync(azureAdUser, cancel);

        if (response != AzureAdResponseStatus.Success)
        {
            throw new DomainException($"Failed to update user details in Azure AD for user ID: {user.Id}. Status: {response}");
        }

        // Sync changes back to local database
        if (hasMobilePhoneChanged)
        {
            user.UpdateMobilePhone(command.MobilePhone ?? string.Empty, _currentUser.Source);
        }

        if (hasNameChanged)
        {
            user.UpdateName(command.FirstName, command.LastName, _currentUser.Source);
        }
    }

    private static bool HasMobilePhoneChanged(string? newMobilePhone, string? currentMobilePhone)
    {
        // Treat null and empty string as equivalent for comparison
        var normalizedNew = string.IsNullOrEmpty(newMobilePhone) ? null : newMobilePhone;
        var normalizedCurrent = string.IsNullOrEmpty(currentMobilePhone) ? null : currentMobilePhone;

        return !string.Equals(normalizedNew, normalizedCurrent, StringComparison.Ordinal);
    }

    private static bool HasNameChanged(string newFirstName, string newLastName, string? currentFirstName, string? currentLastName)
    {
        return !string.Equals(newFirstName, currentFirstName, StringComparison.Ordinal) || !string.Equals(newLastName, currentLastName, StringComparison.Ordinal);
    }

    private async Task PublishUpdateEventNotificationAsync(User user, Office? office, bool hasOfficeAssociation, CancellationToken cancel)
    {
        var action = hasOfficeAssociation ? "User Updated" : "User Invited";
        var message = hasOfficeAssociation
            ? $"User: {user.FirstName} {user.LastName} ({user.Email}) updated in Office: {office!.Name} (OfficeId: {office.Id})."
            : $"User: {user.FirstName} {user.LastName} ({user.Email}) invited to enroll.";

        await _mediator.Publish(
            new IdentityEventLogNotification(
            action,
            _currentUser.Source,
            message,
            _currentUser.UserId,
            user.Id
        ),
            cancel);
    }

    private async Task SendEmailAsync(User user, string inviteUrl, bool isNewUser, bool isExistingOffice, int? officeId, string? officeName, CancellationToken cancel)
    {
        var newUserTemplate = _settingsOptions.EmailTemplates["NewUserEmail"];

        string mobileLastFourDigits = user.MobilePhone?.Length >= 4
            ? user.MobilePhone[^4..]
            : string.Empty;

        var separator = inviteUrl.Contains('?') ? "&" : "?";
        var urlWithParams = officeId is null
            ? $"{inviteUrl}{separator}userId={user.Id}"
            : $"{inviteUrl}{separator}userId={user.Id}&officeId={officeId}";

        var emailMessage = new EmailMessage
        {
            From = new EmailAddress
            {
                Name = newUserTemplate.FromName,
                Address = newUserTemplate.FromAddress
            },
            To = new List<EmailAddress> { new() { Name = user.FullName, Address = user.Email } }.AsReadOnly(),
            TemplateId = newUserTemplate.TemplateId,
            TemplateData = new
            {
                EmailSubject = IdentityConstants.MerchantEmailSubject,
                IsNewOfficeNewUser = !isExistingOffice && isNewUser,
                IsNewOfficeExistingUser = !isExistingOffice && !isNewUser,
                IsExistingOfficeNewUser = isExistingOffice && isNewUser,
                IsExistingOfficeExistingUser = isExistingOffice && !isNewUser,
                InviteUserName = _currentUser.FullName,
                OfficeName = officeName,
                MobileLast4 = isNewUser ? mobileLastFourDigits : null,
                ButtonText = isExistingOffice ? IdentityConstants.MerchantEmailButtonText : IdentityConstants.SuperAdminEmailButtonText,
                ButtonUrl = urlWithParams,
                FirstName = user.FirstName ?? string.Empty
            }
        };

        _ = await _emailProvider.SendAsync(new List<EmailMessage> { emailMessage }.AsReadOnly(), cancel);
    }

    private async Task SendTemporaryPasswordSmsAsync(string userPhoneNumber, string temporaryPassword, CancellationToken cancel)
    {
        // Fetch SMS settings from configuration
        var senderPhoneNumber = _settingsOptions.SmsTemplates["NewUserSms"].FromPhoneNumber;

        // Prepare the SMS message
        var smsBody = $"{temporaryPassword} is your temporary password for w2k sign-in.";

        var smsMessage = new SmsMessage
        {
            From = senderPhoneNumber,
            To = userPhoneNumber,
            Body = smsBody
        };

        _ = await _smsProvider.SendAsync(new List<SmsMessage> { smsMessage }.AsReadOnly(), cancel);
    }
}
