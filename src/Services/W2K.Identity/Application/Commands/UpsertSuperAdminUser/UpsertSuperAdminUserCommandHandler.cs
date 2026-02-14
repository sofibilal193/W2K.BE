using System.ComponentModel.DataAnnotations;
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

public class UpsertSuperAdminUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IEmailProvider emailProvider,
    IOptions<ServiceSettings> settingsOptions,
    ISmsProvider smsProvider,
    IAzureADProvider azureAdProvider)
    : IRequestHandler<UpsertSuperAdminUserCommand, int?>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IEmailProvider _emailProvider = emailProvider ?? throw new ArgumentNullException(nameof(emailProvider));
    private readonly ServiceSettings _settingsOptions = settingsOptions?.Value ?? throw new ArgumentNullException(nameof(settingsOptions));
    private readonly ISmsProvider _smsProvider = smsProvider ?? throw new ArgumentNullException(nameof(smsProvider));
    private readonly IAzureADProvider _azureAdProvider = azureAdProvider ?? throw new ArgumentNullException(nameof(azureAdProvider));

    private string InviteBaseUrl => _settingsOptions.UserInvitationBaseUrl;

    public async Task<int?> Handle(UpsertSuperAdminUserCommand command, CancellationToken cancellationToken)
    {
        var superAdminOfficeId = _currentUser.OfficeIds?.FirstOrDefault()
            ?? throw new NotFoundException("Current user has no office assigned.");

        if (command.UserId is null)
        {
            return await AddSuperAdminUserAsync(command, superAdminOfficeId, cancellationToken);
        }
        else
        {
            await UpdateSuperAdminUserAsync(command, superAdminOfficeId, cancellationToken);
            return command.UserId;
        }
    }

    private async Task<int> AddSuperAdminUserAsync(UpsertSuperAdminUserCommand command, int superAdminOfficeId, CancellationToken cancel)
    {
        // Query office entity if notification is enabled
        var office = await _data.Offices.FirstOrDefaultAsync(x => x.Id == superAdminOfficeId, cancel)
            ?? throw new NotFoundException(nameof(Office), superAdminOfficeId);

        // Call Azure AD provider to add new user to Azure AD B2C
        var azureAdUser = new AzureADUser
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            MobilePhone = command.MobilePhone,
            JobTitle = command.Title,
            LastLoginDateTime = DateTime.UtcNow
        };

        var response = await _azureAdProvider.AddUserAsync(azureAdUser, cancel);

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

            // Associate new user with SuperAdmin Office in B2C
            // IsDefault is always true for SuperAdmin users since they only have one office
            user.UpsertOffice(superAdminOfficeId, command.RoleId, command.Title, isDefault: true);

            _ = await _data.SaveEntitiesAsync(cancel);

            await SendEmailAsync(user, InviteBaseUrl, isNewUser: true, office, cancel);

            // Generate temporary password and send via SMS on creation
            await SendTemporaryPasswordSmsAsync(command.MobilePhone, response.TemporaryPassword, cancel);

            await _mediator.Publish(
                new IdentityEventLogNotification(
                "SuperAdmin User Created",
                _currentUser.Source,
                $"SuperAdmin User: {user.FirstName} {user.LastName} ({user.Email}) added to SuperAdmin Office (OfficeId: {superAdminOfficeId}).",
                _currentUser.UserId,
                user.Id),
                cancel);

            return user.Id;
        }
        else
        {
            throw new ValidationException($"Failed to create user in Azure AD for email: {command.Email}. Status: {response.Status}");
        }
    }

    private async Task UpdateSuperAdminUserAsync(UpsertSuperAdminUserCommand command, int superAdminOfficeId, CancellationToken cancel)
    {
        var user = await _data.Users.Include(x => x.Offices).FirstOrDefaultAsync(x => x.Id == command.UserId, cancel)
            ?? throw new NotFoundException(nameof(User), command.UserId!.Value);

        // Update role and title only (office cannot be changed for SuperAdmin users)
        var existingOffice = user.Offices.FirstOrDefault(x => x.OfficeId == superAdminOfficeId)
            ?? throw new NotFoundException($"User is not associated with SuperAdmin office (OfficeId: {superAdminOfficeId}).");

        existingOffice.Update(true, command.RoleId, command.Title);

        // Update user details in Azure AD when edited (First Name, Last Name, Email, Mobile, Role)
        await UpdateUserInAzureAdAsync(user, command, cancel);

        _ = await _data.SaveEntitiesAsync(cancel);

        await _mediator.Publish(
            new IdentityEventLogNotification(
            "SuperAdmin User Updated",
            _currentUser.Source,
            $"SuperAdmin User: {user.FirstName} {user.LastName} ({user.Email}) updated.",
            _currentUser.UserId,
            user.Id
        ),
            cancel);
    }

    private async Task UpdateUserInAzureAdAsync(User user, UpsertSuperAdminUserCommand command, CancellationToken cancel)
    {
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

        var response = await _azureAdProvider.UpdateUserAsync(azureAdUser, cancel);

        if (response != AzureAdResponseStatus.Success)
        {
            throw new ValidationException($"Failed to update user details in Azure AD for user ID: {user.Id}. Status: {response}");
        }

        // Sync changes back to local database
        if (hasMobilePhoneChanged)
        {
            user.UpdateMobilePhone(command.MobilePhone, _currentUser.Source);
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
        return !string.Equals(newFirstName, currentFirstName, StringComparison.Ordinal)
            || !string.Equals(newLastName, currentLastName, StringComparison.Ordinal);
    }

    private async Task SendEmailAsync(User user, string inviteUrl, bool isNewUser, Office office, CancellationToken cancel)
    {
        var newUserTemplate = _settingsOptions.EmailTemplates["NewUserEmail"];

        string mobileLastFourDigits = user.MobilePhone?.Length >= 4
            ? user.MobilePhone[^4..]
            : string.Empty;

        var separator = inviteUrl.Contains('?') ? "&" : "?";
        var urlWithParams = $"{inviteUrl}{separator}userId={user.Id}&officeId={office.Id}";

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
                IsNewOfficeNewUser = false,
                IsNewOfficeExistingUser = false,
                IsExistingOfficeNewUser = isNewUser,
                IsExistingOfficeExistingUser = !isNewUser,
                InviteUserName = _currentUser.FullName,
                OfficeName = office.Name,
                MobileLast4 = isNewUser ? mobileLastFourDigits : null,
                ButtonText = IdentityConstants.MerchantEmailButtonText,
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
