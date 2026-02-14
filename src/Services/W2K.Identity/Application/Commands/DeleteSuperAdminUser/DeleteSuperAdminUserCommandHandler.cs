using W2K.Common.Application.AzureAd;
using W2K.Common.Application.Validations;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

namespace W2K.Identity.Application.Commands.DeleteSuperAdminUser;

public class DeleteSuperAdminUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IAzureADProvider azureAdProvider,
    ILogger<DeleteSuperAdminUserCommandHandler> logger)
    : IRequestHandler<DeleteSuperAdminUserCommand>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IAzureADProvider _azureAdProvider = azureAdProvider ?? throw new ArgumentNullException(nameof(azureAdProvider));
    private readonly ILogger<DeleteSuperAdminUserCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(DeleteSuperAdminUserCommand command, CancellationToken cancellationToken)
    {
        // Step 1: Validate that the current user is not deleting themselves
        ValidateNotSelfDelete(command.UserId);

        // Step 2: Load user with office associations using eager loading
        var user = await LoadUserWithOfficesAsync(command.UserId, cancellationToken);

        // Step 3: Validate ProviderId exists (required for B2C deletion)
        ValidateProviderId(user);

        // Step 4: Delete from Azure AD B2C first (external-first pattern)
        await DeleteFromAzureAdAsync(user, cancellationToken);

        // Step 5: Remove user entity (cascade deletes OfficeUser associations)
        _data.Users.Remove(user);

        // Save changes and publish audit event
        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await PublishDeletionNotificationAsync(user, cancellationToken);

        _logger.LogInformation(
            "SuperAdmin user deleted successfully. UserId: {UserId}, DeletedBy: {ActorUserId}",
            user.Id,
            _currentUser.UserId);
    }

    private void ValidateNotSelfDelete(int userId)
    {
        if (userId == _currentUser.UserId)
        {
            throw ValidationCodes.GenerateValidationException(
                propertyName: "",
                validationCode: ValidationCodes.SuperAdminDeleteSelfNotAllowed);
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

    private static void ValidateProviderId(User user)
    {
        if (string.IsNullOrWhiteSpace(user.ProviderId))
        {
            throw ValidationCodes.GenerateValidationException(
                propertyName: nameof(user.ProviderId),
                validationCode: ValidationCodes.UserProviderIdRequiredToDelete,
                propertyValue: user.Id.ToString());
        }
    }

    private async Task DeleteFromAzureAdAsync(User user, CancellationToken cancel)
    {
        var result = await _azureAdProvider.DeleteUserAsync(user.ProviderId!, cancel);

        if (result == AzureAdResponseStatus.Notfound)
        {
            _logger.LogWarning(
                "User not found in Azure AD during deletion. UserId: {UserId}, ProviderId: {ProviderId}",
                user.Id,
                user.ProviderId);

            return;
        }

        if (result != AzureAdResponseStatus.Success)
        {
            _logger.LogError(
                "Failed to delete SuperAdmin user in Azure AD. UserId: {UserId}, ProviderId: {ProviderId}, Status: {Status}",
                user.Id,
                user.ProviderId,
                result);

            throw new DomainException(
                $"Failed to delete SuperAdmin user in Azure AD. ProviderId: {user.ProviderId}, UserId: {user.Id}, Status: {result}");
        }

        _logger.LogInformation(
            "SuperAdmin user deleted from Azure AD. UserId: {UserId}, ProviderId: {ProviderId}",
            user.Id,
            user.ProviderId);
    }

    private async Task PublishDeletionNotificationAsync(
        User user,
        CancellationToken cancel)
    {
        var message = $"SuperAdmin User deleted. UserId: {user.Id}.";

        var notification = new IdentityEventLogNotification(
            eventType: "SuperAdmin User Deleted.",
            source: _currentUser.Source,
            description: message,
            userId: _currentUser.UserId,
            officeId: user.Offices.First().OfficeId);

        await _mediator.Publish(notification, cancel);
    }
}
