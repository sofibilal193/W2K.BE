using W2K.Common.Application.AzureAd;
using W2K.Common.Application.Validations;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ActivateSuperAdminUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IAzureADProvider azureAdProvider,
    ILogger<ActivateSuperAdminUserCommandHandler> logger) : IRequestHandler<ActivateSuperAdminUserCommand>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IAzureADProvider _azureAdProvider = azureAdProvider ?? throw new ArgumentNullException(nameof(azureAdProvider));
    private readonly ILogger<ActivateSuperAdminUserCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(ActivateSuperAdminUserCommand command, CancellationToken cancellationToken)
    {
        // Step 1: Validate that the current user is not deleting themselves
        ValidateNotSelfActivateDeactivate(command.UserId);

        // Step 2: Load user with office associations using eager loading
        var user = await LoadUserWithOfficesAsync(command.UserId, cancellationToken);

        // Step 3: Validate user has SuperAdmin office association
        ValidateSuperAdminOfficeAssociation(user);

        // Step 4: Toggle status in Azure AD B2C (if ProviderId exists)
        await UpdateB2CAsync(user, command.IsActive, cancellationToken);

        // Step 5: Update local database
        UpdateLocalDbAsync(user, command.IsActive);

        // Step 6: Save changes and publish audit event
        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await PublishActivationDeactivationNotificationAsync(user, command, cancellationToken);
    }

    private void ValidateNotSelfActivateDeactivate(int userId)
    {
        if (userId == _currentUser.UserId)
        {
            throw ValidationCodes.GenerateValidationException(
                propertyName: "",
                validationCode: ValidationCodes.SuperAdminActivateDeactivateSelfNotAllowed);
        }
    }

    private async Task<User> LoadUserWithOfficesAsync(int userId, CancellationToken cancel)
    {
        var user = await _data.Users
            .Include(x => x.Offices)
            .Include("Offices.Office")
            .GetAsync(userId, cancel);

        return user ?? throw new NotFoundException(nameof(User), userId);
    }

    private static void ValidateSuperAdminOfficeAssociation(User user)
    {
        if (!user.Offices.Any(x => x.Office?.Type == OfficeType.SuperAdmin))
        {
            throw new NotFoundException($"User {user.Id} is not associated with a SuperAdmin office.");
        }
    }

    private async Task UpdateB2CAsync(User user, bool isActive, CancellationToken cancel)
    {
        // Skip Azure AD B2C update if ProviderId is missing
        if (string.IsNullOrWhiteSpace(user.ProviderId))
        {
            _logger.LogInformation(
                "Skipping Azure AD B2C update - no ProviderId found. UserId: {UserId}, IsActive: {IsActive}. Updating local DB only.",
                user.Id,
                isActive);
            return;
        }

        // Toggle user status in Azure AD B2C
        var azureAdResult = await _azureAdProvider.ToggleUserStatusAsync(user.ProviderId, isActive, cancel);
        if (azureAdResult == AzureAdResponseStatus.Notfound)
        {
            _logger.LogWarning(
                "User not found in Azure AD during status toggle. UserId: {UserId}, ProviderId: {ProviderId}. Proceeding with local update.",
                user.Id,
                user.ProviderId);
            // Continue with local update - user may have been manually removed from B2C
            return;
        }

        if (azureAdResult != AzureAdResponseStatus.Success)
        {
            _logger.LogError(
                "Failed to toggle SuperAdmin user status in Azure AD. UserId: {UserId}, ProviderId: {ProviderId}, Status: {Status}",
                user.Id,
                user.ProviderId,
                azureAdResult);

            throw ValidationCodes.GenerateValidationException(
                propertyName: nameof(user.ProviderId),
                validationCode: ValidationCodes.FailedToUpdateUserInB2C,
                propertyValue: user.ProviderId);
        }

        _logger.LogInformation(
            "SuperAdmin B2C User status toggled successfully. UserId: {UserId}, ProviderId: {ProviderId}, IsActive: {IsActive}",
            user.Id,
            user.ProviderId,
            isActive);
    }

    private static void UpdateLocalDbAsync(User user, bool isActive)
    {
        // Get SuperAdmin office association
        var superAdminOfficeAssociation = user.Offices.First(x => x.Office?.Type == OfficeType.SuperAdmin);

        // Apply status changes locally
        if (isActive)
        {
            user.Enable();
            superAdminOfficeAssociation.Enable();
        }
        else
        {
            user.Disable();
            superAdminOfficeAssociation.Disable();
        }
    }

    private async Task PublishActivationDeactivationNotificationAsync(
        User user,
        ActivateSuperAdminUserCommand command,
        CancellationToken cancel)
    {
        var superAdminOfficeAssociation = user.Offices.First(x => x.Office?.Type == OfficeType.SuperAdmin);

        // Log the action
        var action = command.IsActive ? "SuperAdmin User Activated" : "SuperAdmin User Deactivated";
        var b2cUpdateText = string.IsNullOrWhiteSpace(user.ProviderId) ? "" : " and Azure AD B2C";
        var message = $"SuperAdmin UserId: {user.Id} {(command.IsActive ? "activated" : "deactivated")} in SA Portal{b2cUpdateText}. OfficeId: {superAdminOfficeAssociation.OfficeId}";

        var notification = new IdentityEventLogNotification(
            action,
            _currentUser.Source,
            message,
            _currentUser.UserId,
            user.Id);


        await _mediator.Publish(notification, cancel);
    }
}
