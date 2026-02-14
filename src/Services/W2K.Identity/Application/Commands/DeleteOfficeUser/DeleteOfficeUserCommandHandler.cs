using W2K.Common.Application.AzureAd;
using W2K.Common.Application.Validations;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130

public class DeleteOfficeUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IAzureADProvider azureAdProvider,
    ILogger<DeleteOfficeUserCommandHandler> logger)
    : IRequestHandler<DeleteOfficeUserCommand>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IAzureADProvider _azureAdProvider = azureAdProvider ?? throw new ArgumentNullException(nameof(azureAdProvider));
    private readonly ILogger<DeleteOfficeUserCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(DeleteOfficeUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId == _currentUser.UserId)
        {
            throw ValidationCodes.GenerateValidationException(
                propertyName: "",
                validationCode: ValidationCodes.DeleteSelfNotPermitted);
        }

        var user = await LoadUserWithOfficesAsync(command.UserId, cancellationToken);
        var isSuperAdmin = _currentUser.OfficeType == OfficeType.SuperAdmin;

        ValidateSuperAdminProtection(user, isSuperAdmin);

        var deletionScope = DetermineDeletionScope(
            command.OfficeId,
            [.. user.Offices],
            isSuperAdmin,
            command.UserId);

        var targetOfficeUser = deletionScope.TargetOfficeUser;
        var isFullUserDeletion = deletionScope.IsFullUserDeletion;
        var affectedOfficeIds = deletionScope.AffectedOfficeIds;

        if (isFullUserDeletion)
        {
            await DeleteFromAzureAdAsync(user, cancellationToken);
        }

        RemoveOfficeAssociations(user, affectedOfficeIds, targetOfficeUser);

        if (isFullUserDeletion)
        {
            user.Disable();
            _data.Users.Remove(user);
        }
        else
        {
            // If user still has active offices after deletion, ensure user entity is enabled
            var hasRemainingActiveOffices = user.Offices.Any(x => !x.IsDisabled);
            if (hasRemainingActiveOffices && user.IsDisabled)
            {
                user.Enable();
                _logger.LogInformation(
                    "User re-enabled after deleting office. UserId: {UserId} has {RemainingOfficeCount} remaining active office(s)",
                    user.Id,
                    user.Offices.Count(x => !x.IsDisabled));
            }
        }

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await PublishDeletionNotificationAsync(
            user,
            targetOfficeUser,
            isFullUserDeletion,
            affectedOfficeIds,
            isSuperAdmin,
            command.OfficeId,
            cancellationToken);

        _logger.LogInformation(
            "User deletion completed. UserId: {UserId}, FullDeletion: {IsFullDeletion}, AffectedOffices: [{OfficeIds}], DeletedBy: {ActorUserId}",
            user.Id,
            isFullUserDeletion,
            string.Join(", ", affectedOfficeIds),
            _currentUser.UserId);
    }

    private async Task<User> LoadUserWithOfficesAsync(int userId, CancellationToken cancel)
    {
        return await _data.Users
                .Include(x => x.Offices)
                .Include("Offices.Office")
                .GetAsync(userId, cancel)
            ?? throw new NotFoundException(nameof(User), userId);
    }

    private static void ValidateSuperAdminProtection(User user, bool isSuperAdminCaller)
    {
        var hasSuperAdminOffices = user.Offices.Any(x => x.Office?.Type == OfficeType.SuperAdmin);

        if (hasSuperAdminOffices)
        {
            var errorMessage = isSuperAdminCaller
                ? "Superadmin users require elevated permissions to delete. Use the Superadmin section for such actions."
                : "You do not have permission to delete a user with SuperAdmin office associations.";

            throw ValidationCodes.GenerateValidationException(
                propertyName: "",
                validationCode: ValidationCodes.SuperAdminUserDeletion,
                errorMessage: errorMessage
                        );
        }
    }

    private static DeletionScope DetermineDeletionScope(
        int? officeId,
        IList<OfficeUser> officeUserAssociations,
        bool isSuperAdmin,
        int userId)
    {
        if (isSuperAdmin)
        {
            if (officeId.HasValue)
            {
                // SuperAdmin deleting from specific office
                var targetOfficeUser = officeUserAssociations.FirstOrDefault(x => x.OfficeId == officeId.Value)
                    ?? throw new NotFoundException($"{nameof(OfficeUser)} not found for UserId: {userId} and OfficeId: {officeId}");

                // Count active offices EXCLUDING the one being deleted
                var remainingActiveOfficeCount = officeUserAssociations.Count(x => !x.IsDisabled && x.OfficeId != officeId.Value);
                var isFullDeletion = remainingActiveOfficeCount == 0;

                return new DeletionScope(targetOfficeUser, isFullDeletion, [officeId.Value]);
            }
            else
            {
                // SuperAdmin deleting from all offices
                var allOfficeIds = officeUserAssociations.Select(x => x.OfficeId).ToList();
                return new DeletionScope(null, true, allOfficeIds);
            }
        }
        else
        {
            // Non-SuperAdmin deleting from their office
            var targetOfficeUser = officeUserAssociations.FirstOrDefault(x => x.OfficeId == officeId!.Value)
                ?? throw new NotFoundException($"{nameof(OfficeUser)} not found for UserId: {userId} and OfficeId: {officeId}");

            // Count remaining active offices EXCLUDING the one being deleted
            var remainingActiveOfficeCount = officeUserAssociations.Count(x => !x.IsDisabled && x.OfficeId != officeId);
            var isFullDeletion = remainingActiveOfficeCount == 0;

            return new DeletionScope(targetOfficeUser, isFullDeletion, [officeId!.Value]);
        }
    }

    private async Task DeleteFromAzureAdAsync(User user, CancellationToken cancel)
    {
        if (string.IsNullOrWhiteSpace(user.ProviderId))
        {
            return;
        }

        var result = await _azureAdProvider.DeleteUserAsync(user.ProviderId!, cancel);
        if (result != AzureAdResponseStatus.Success)
        {
            _logger.LogError(
                "Failed to delete user in Azure AD. UserId: {UserId}, Status: {Status}",
                user.Id,
                result);

            throw new DomainException($"Failed to delete user in Azure AD for UserId: {user.Id}. Status: {result}");
        }

        _logger.LogInformation(
            "User deleted from Azure AD. UserId: {UserId}",
            user.Id);
    }

    private void RemoveOfficeAssociations(User user, List<int> affectedOfficeIds, OfficeUser? targetOfficeUser)
    {
        if (affectedOfficeIds.Count > 1 || targetOfficeUser is null)
        {
            // Remove all specified offices
            foreach (var officeId in affectedOfficeIds)
            {
                user.RemoveOffice(officeId);
            }
        }
        else
        {
            // Remove single office
            var isDeletingDefaultOffice = targetOfficeUser.IsDefault;

            user.RemoveOffice(targetOfficeUser.OfficeId);

            // If we're deleting the default office and user still has other active offices,
            // reassign default to another office
            if (isDeletingDefaultOffice)
            {
                var remainingActiveOffices = user.Offices
                    .Where(x => !x.IsDisabled && x.OfficeId != targetOfficeUser.OfficeId)
                    .OrderBy(x => x.CreateDateTimeUtc)
                    .ToList();

                if (remainingActiveOffices.Count > 0)
                {
                    // Find the next office in creation order after the deleted office, or wrap around to first
                    var nextDefaultOffice = remainingActiveOffices.FirstOrDefault(x => x.CreateDateTimeUtc > targetOfficeUser.CreateDateTimeUtc)
                        ?? remainingActiveOffices.FirstOrDefault();

                    if (nextDefaultOffice is not null)
                    {
                        nextDefaultOffice.SetIsDefault();

                        _logger.LogInformation(
                            "Default office reassigned for UserId: {UserId} from deleted OfficeId: {DeletedOfficeId} to OfficeId: {NewDefaultOfficeId}",
                            user.Id,
                            targetOfficeUser.OfficeId,
                            nextDefaultOffice.OfficeId);
                    }
                }
            }
        }
    }

    private async Task PublishDeletionNotificationAsync(
        User user,
        OfficeUser? targetOfficeUser,
        bool isFullUserDeletion,
        List<int> affectedOfficeIds,
        bool isSuperAdmin,
        int? requestedOfficeId,
        CancellationToken cancel)
    {
        var action = isFullUserDeletion ? "User Deleted." : "User Removed From Office.";
        var message = BuildNotificationMessage(
            user,
            targetOfficeUser,
            isFullUserDeletion,
            affectedOfficeIds,
            isSuperAdmin,
            requestedOfficeId);

        var notification = new IdentityEventLogNotification(
            action,
            _currentUser.Source,
            message,
            _currentUser.UserId,
            user.Id);

        await _mediator.Publish(notification, cancel);
    }

    private static string BuildNotificationMessage(
        User user,
        OfficeUser? targetOfficeUser,
        bool isFullUserDeletion,
        List<int> affectedOfficeIds,
        bool isSuperAdmin,
        int? requestedOfficeId)
    {
        var userInfo = $"UserId: {user.Id}, User: {user.FirstName} {user.LastName}";

        if (isSuperAdmin && !requestedOfficeId.HasValue)
        {
            // SuperAdmin deleting from all offices
            var allOfficeIds = string.Join(", ", affectedOfficeIds);
            return $"{userInfo} deleted by SuperAdmin; removed from Offices: [{allOfficeIds}].";
        }

        if (isSuperAdmin && requestedOfficeId.HasValue)
        {
            // SuperAdmin deleting from specific office
            var officeInfo = $"OfficeId: {targetOfficeUser?.OfficeId} (Office: {targetOfficeUser?.Office?.Name})";
            return isFullUserDeletion
                ? $"{userInfo} deleted by SuperAdmin after removing last office association from {officeInfo}."
                : $"{userInfo} removed by SuperAdmin from {officeInfo}.";
        }

        // Non-SuperAdmin deleting from their office
        var merchantOfficeInfo = $"OfficeId: {targetOfficeUser!.OfficeId} (Office: {targetOfficeUser?.Office?.Name})";
        return isFullUserDeletion
            ? $"{userInfo} deleted after removing last office association from {merchantOfficeInfo}."
            : $"{userInfo} removed from {merchantOfficeInfo}.";
    }

    private sealed record DeletionScope(
        OfficeUser? TargetOfficeUser,
        bool IsFullUserDeletion,
        List<int> AffectedOfficeIds);
}
