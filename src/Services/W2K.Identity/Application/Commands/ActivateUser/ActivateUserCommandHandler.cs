using W2K.Identity.Repositories;
using W2K.Identity.Application.Notifications;
using W2K.Common.Application.AzureAd;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Entities;
using W2K.Common.Application.Validations;
using W2K.Identity.Application.Events;

namespace W2K.Identity.Application.Commands.ActivateUser;

public class ActivateUserCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator,
    IAzureADProvider azureAdProvider) : IRequestHandler<ActivateUserCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;
    private readonly IAzureADProvider _azureAdProvider = azureAdProvider;

    public async Task Handle(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId == _currentUser.UserId)
        {
            throw ValidationCodes.GenerateValidationException(
                propertyName: "",
                validationCode: ValidationCodes.ActivateDeactivateSelfNotPermitted);
        }
        // Load user with office associations and office types (typed includes for safety)
        // Repository abstraction does not support ThenInclude; use string path for nested navigation.
        var user = await _data.Users
                    .Include(x => x.Offices)
                    .Include("Offices.Office")
                    .GetAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), command.UserId);

        var officeUserAssociations = user.Offices;
        var isSuperAdminCaller = _currentUser.OfficeType == OfficeType.SuperAdmin;
        var hasProviderId = !string.IsNullOrWhiteSpace(user.ProviderId);

        // Validate permissions and get target office user
        ValidateCallerPermissions(isSuperAdminCaller, command.OfficeId);
        var targetOfficeUser = GetAndValidateTargetOfficeUser(command.OfficeId, officeUserAssociations, isSuperAdminCaller);
        ValidateSuperAdminOperations(isSuperAdminCaller, officeUserAssociations);

        // Compute active association metrics (cache to avoid multiple enumerations)
        var activeAssociations = officeUserAssociations.Where(x => !x.IsDisabled).ToList();
        var activeCount = activeAssociations.Count;
        var wasFullyDisabled = user.IsDisabled && activeCount == 0;
        var isDeactivatingLastActiveOffice = !command.IsActive && command.OfficeId.HasValue && activeCount == 1 && activeAssociations[0].OfficeId == command.OfficeId.Value;
        var isActivatingFullyDisabledUser = command.IsActive && wasFullyDisabled;
        var isGlobalToggle = isSuperAdminCaller && !command.OfficeId.HasValue;

        bool isFullToggle = isGlobalToggle || isDeactivatingLastActiveOffice || isActivatingFullyDisabledUser;

        // Toggle Azure AD only on full user activation/deactivation and when ProviderId exists
        if (isFullToggle && hasProviderId)
        {
            var result = await _azureAdProvider.ToggleUserStatusAsync(user.ProviderId!, command.IsActive, cancellationToken);
            if (result != AzureAdResponseStatus.Success)
            {
                throw new DomainException($"Failed to update user status in Azure AD. Status: {result}");
            }
        }

        // Apply status changes
        if (isSuperAdminCaller && !command.OfficeId.HasValue)
        {
            // SuperAdmin without OfficeId: Apply to all offices globally
            foreach (var assoc in officeUserAssociations)
            {
                assoc.SetActiveStatus(command.IsActive);
            }

            if (command.IsActive)
            {
                user.Enable();

                // Only set a default if none exists
                var hasNoActiveDefault = !officeUserAssociations.Any(x => x.IsDefault && !x.IsDisabled);
                if (hasNoActiveDefault)
                {
                    var firstActiveOffice = officeUserAssociations.FirstOrDefault(x => !x.IsDisabled);
                    firstActiveOffice?.SetIsDefault();
                }
            }
            else
            {
                user.Disable();
            }
        }
        else
        {
            // Either non-SuperAdmin OR SuperAdmin with specific OfficeId: Apply to specific office only
            var isDeactivatingDefaultOffice = !command.IsActive && targetOfficeUser!.IsDefault;
            var hasNoActiveDefault = !officeUserAssociations.Any(x => x.IsDefault && !x.IsDisabled);

            targetOfficeUser!.SetActiveStatus(command.IsActive);

            // If activating and no default office exists, make this the default
            if (command.IsActive && hasNoActiveDefault)
            {
                targetOfficeUser!.SetIsDefault();
            }
            // If deactivating the default office, clear its default flag and reassign to another active office
            else if (isDeactivatingDefaultOffice)
            {
                targetOfficeUser!.ClearIsDefault();

                var newDefaultOffice = officeUserAssociations
                    .FirstOrDefault(x => x.OfficeId != targetOfficeUser.OfficeId && !x.IsDisabled);

                newDefaultOffice?.SetIsDefault();
                newDefaultOffice?.ProcessInvite();
            }

            // If deactivating the last active office, disable user entity
            // (activeCount was computed before ApplyStatus, so activeCount==1 means we're deactivating the last one)
            if (!command.IsActive && activeCount == 1)
            {
                user.Disable();
            }
            // If activating when user is disabled, enable user entity
            // (Either activating first office OR re-activating after being disabled)
            else if (command.IsActive && user.IsDisabled)
            {
                user.Enable();
            }
        }
        _ = await _data.SaveEntitiesAsync(cancellationToken);

        // Build notification
        var action = command.IsActive ? "User Activated" : "User Deactivated";
        string message;
        if (isSuperAdminCaller && !command.OfficeId.HasValue)
        {
            var officeIds = string.Join(", ", officeUserAssociations.Select(x => x.OfficeId));
            message = $"UserId: {user.Id} {action} by SuperAdmin across all Offices: [{officeIds}].";
        }
        else
        {
            message = $"UserId: {user.Id} {action} in OfficeId: {targetOfficeUser!.OfficeId} (Office: {targetOfficeUser.Office?.Name}).";
        }

        var notification = new IdentityEventLogNotification(
            action,
            _currentUser.Source,
            message,
            _currentUser.UserId,
            user.Id);

        await _mediator.Publish(notification, cancellationToken);
    }

    private static void ValidateCallerPermissions(bool isSuperAdminCaller, int? officeId)
    {
        if (isSuperAdminCaller)
        {
            return;
        }

        if (!officeId.HasValue)
        {
            throw new DomainException("OfficeId is required when a non-SuperAdmin attempts to activate or deactivate a user.");
        }
    }

    private static OfficeUser? GetAndValidateTargetOfficeUser(
        int? officeId,
        IReadOnlyCollection<OfficeUser> officeUserAssociations,
        bool isSuperAdminCaller)
    {
        if (!officeId.HasValue)
        {
            return null;
        }

        var targetOfficeUser = officeUserAssociations.FirstOrDefault(x => x.OfficeId == officeId.Value)
            ?? throw new NotFoundException(nameof(OfficeUser), officeId.Value);

        return !isSuperAdminCaller && targetOfficeUser.Office?.Type == OfficeType.SuperAdmin
            ? throw new DomainException("Activation or deactivation of SuperAdmin users is not permitted (target office association).")
            : targetOfficeUser;
    }

    private static void ValidateSuperAdminOperations(bool isSuperAdminCaller, IReadOnlyCollection<OfficeUser> officeUserAssociations)
    {
        if (isSuperAdminCaller)
        {
            return;
        }

        if (officeUserAssociations.Any(x => x.Office?.Type == OfficeType.SuperAdmin))
        {
            throw new DomainException("Activation or deactivation affecting a SuperAdmin association requires SuperAdmin privileges.");
        }
    }
}
