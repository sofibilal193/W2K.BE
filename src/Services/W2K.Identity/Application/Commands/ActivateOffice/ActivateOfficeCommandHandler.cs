#pragma warning disable CA2201 // Do not raise reserved exception types
using W2K.Identity.Repositories;
using W2K.Identity.Application.Notifications;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Entities;
using W2K.Common.Application.AzureAd;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ActivateOfficeCommandHandler(
    IIdentityUnitOfWork data,
    IMediator mediator,
    ICurrentUser currentUser,
    IAzureADProvider azureDbProvider,
    ILogger<ActivateOfficeCommandHandler> logger
) : IRequestHandler<ActivateOfficeCommand>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAzureADProvider _azureDbProvider = azureDbProvider;
    private readonly ILogger<ActivateOfficeCommandHandler> _logger = logger;

    public async Task Handle(ActivateOfficeCommand command, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.GetAsync(command.OfficeId, cancellationToken)
            ?? throw new NotFoundException($"Office not found for OfficeId: {command.OfficeId}");

        CheckAccessControl(office, command.OfficeId);

        foreach (var officeUser in office.Users)
        {
            var hasOtherActiveOffices = await _data.OfficeUsers
                .AnyAsync(x => x.UserId == officeUser.UserId && x.OfficeId != command.OfficeId && !x.IsDisabled, cancellationToken);

            await ProcessOfficeUserAsync(officeUser, office, hasOtherActiveOffices, command.IsActive, cancellationToken);
        }

        UpdateOfficeStatus(office, command.IsActive);

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        await LogOfficeEventAsync(office, command.IsActive, command.OfficeId, cancellationToken);
    }

    private async Task ProcessOfficeUserAsync(OfficeUser officeUser, Office office, bool hasOtherActiveOffices, bool isActive, CancellationToken cancellation)
    {
        if (officeUser is null)
        {
            return;
        }

        var user = officeUser.User;
        var providerId = user?.ProviderId;

        // Only toggle Azure AD when transitioning the global enabled/disabled state (no other active office associations)
        if (!hasOtherActiveOffices && providerId is not null)
        {
            var azureStatus = await _azureDbProvider.ToggleUserStatusAsync(providerId, isActive, cancellation);
            if (azureStatus != AzureAdResponseStatus.Success)
            {
                var ex = new Exception($"Failed to update user status in Azure AD. Status: {azureStatus} for ProviderId: {providerId}");
                _logger.LogError(
                    ex,
                    "Office user status update failed in Azure AD. Status: {AzureStatus}, ProviderId: {ProviderId}, OfficeId: {OfficeId}, UserId: {UserId}",
                    azureStatus,
                    providerId,
                    officeUser.OfficeId,
                    officeUser.UserId
                );
            }

            if (user is not null)
            {
                if (isActive)
                {
                    user.Enable();
                }
                else
                {
                    user.Disable();
                }
            }
        }
        else if (isActive && providerId is not null)
        {
            // Multi-office scenario: ensure local user enabled (directory assumed already enabled)
            user?.Enable();
        }

        // Update office-user association state
        if (isActive)
        {
            officeUser.Enable();

            // If activating and user has no active default office, set this one as default
            if (user is not null)
            {
                await SetDefaultOfficeOnActivationAsync(user, officeUser, cancellation);
            }
        }
        else
        {
            officeUser.Disable();

            // If disabling the user's default office
            if (officeUser.IsDefault && user is not null)
            {
                // Clear the default flag from this office
                officeUser.Update(isDefault: false, officeUser.RoleId, officeUser.Title);

                if (hasOtherActiveOffices)
                {
                    // User has other active offices, set the next one as default
                    await SetNextOfficeAsDefaultAsync(user, officeUser.OfficeId, cancellation);
                }
                else
                {
                    _logger.LogInformation(
                        "Cleared default flag for OfficeId: {OfficeId} for UserId: {UserId} (office deactivated)",
                        officeUser.OfficeId,
                        user.Id
                    );
                }
            }
        }

        await LogUserEventAsync(user, office, hasOtherActiveOffices, isActive, cancellation);
    }

    private async Task SetDefaultOfficeOnActivationAsync(User user, OfficeUser activatingOfficeUser, CancellationToken cancellation)
    {
        // Get all offices for the user (EXCLUDING the one being activated to avoid EF change tracking issues)
        // We need to check the database state, not the in-memory tracked state
        // Sorted by CreateDateTimeUtc (the order they were added to offices)
        var allOtherOffices = await _data.OfficeUsers.GetAsync(
            predicate: x => x.UserId == user.Id && x.OfficeId != activatingOfficeUser.OfficeId && !x.IsDisabled,
            sortKeySelector: x => x.CreateDateTimeUtc,  // Sort by creation timestamp (when user was added to office)
            sortDescending: false,
            cancel: cancellation
        );

        // Check if user has any other active (non-disabled) default office
        var hasActiveDefault = allOtherOffices.Any(x => !x.IsDisabled && x.IsDefault);

        // If no active default office exists, set the activating office as default
        if (!hasActiveDefault)
        {
            // First, clear IsDefault flag from ALL other offices (active or disabled) to maintain single default
            foreach (var otherOffice in allOtherOffices.Where(x => x.IsDefault))
            {
                otherOffice.Update(isDefault: false, otherOffice.RoleId, otherOffice.Title);
            }

            activatingOfficeUser.SetIsDefault();

            _logger.LogInformation(
                "Set OfficeId: {OfficeId} as default for UserId: {UserId} during activation (no other active default office)",
                activatingOfficeUser.OfficeId,
                user.Id
            );
        }
    }

    private async Task SetNextOfficeAsDefaultAsync(User user, int disabledOfficeId, CancellationToken cancellation)
    {
        // Get the disabled office user to find its creation timestamp
        var disabledOfficeUser = await _data.OfficeUsers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.OfficeId == disabledOfficeId,
            cancellation
        );

        if (disabledOfficeUser is null)
        {
            _logger.LogWarning(
                "Could not find disabled office user for UserId: {UserId}, OfficeId: {OfficeId}",
                user.Id,
                disabledOfficeId
            );
            return;
        }

        // Get all active offices for the user (excluding the one being disabled), sorted by CreateDateTimeUtc (the order they were added)
        var activeOffices = await _data.OfficeUsers.GetAsync(
            predicate: x => x.UserId == user.Id && x.OfficeId != disabledOfficeId && !x.IsDisabled,
            sortKeySelector: x => x.CreateDateTimeUtc,  // Sort by creation timestamp (when user was added to office)
            sortDescending: false,
            cancel: cancellation
        );

        if (activeOffices.Count == 0)
        {
            return; // No other active offices
        }

        // Try to find the next office in the order they were added (CreateDateTimeUtc > disabledOfficeUser.CreateDateTimeUtc)
        // Otherwise, wrap around to the first office (circular behavior)
        var nextDefaultOffice = activeOffices.FirstOrDefault(x => x.CreateDateTimeUtc > disabledOfficeUser.CreateDateTimeUtc) ?? activeOffices.FirstOrDefault();

        if (nextDefaultOffice is not null)
        {
            // Clear IsDefault from all other offices first
            foreach (var officeUser in activeOffices.Where(x => x.IsDefault && x.Id != nextDefaultOffice.Id))
            {
                officeUser.Update(isDefault: false, officeUser.RoleId, officeUser.Title);
            }

            nextDefaultOffice.SetIsDefault();

            _logger.LogInformation(
                "Default office changed for UserId: {UserId} from OfficeId: {DisabledOfficeId} to OfficeId: {NewDefaultOfficeId}",
                user.Id,
                disabledOfficeId,
                nextDefaultOffice.OfficeId
            );
        }
    }

    private void CheckAccessControl(Office office, int officeId)
    {
        if (_currentUser.OfficeType == OfficeType.Merchant)
        {
            var isAssigned = office.Users.Any(x => x.UserId == _currentUser.UserId);
            if (!isAssigned)
            {
                throw new DomainException($"User {_currentUser.UserId} is not assigned to OfficeId: {officeId}");
            }
        }
    }

    private static void UpdateOfficeStatus(Office office, bool isActive)
    {
        office.SetActiveStatus(isActive);
    }

    private async Task LogUserEventAsync(User? user, Office office, bool hasOtherActiveOffices, bool isActive, CancellationToken cancel)
    {
        var userAction = isActive ? "User Activated" : "User Deactivated";
        var userMessage = $"User: {user?.FirstName} {user?.LastName} ({user?.Email}) " + (hasOtherActiveOffices ? $"{userAction} in OfficeId: {office.Id} (Office: {office.Name})." : $"{userAction} after updating last office association from OfficeId: {office.Id} (Office: {office.Name}).");

        var userNotification = new IdentityEventLogNotification(
            userAction,
            _currentUser.Source,
            userMessage,
            _currentUser.UserId,
            user?.Id
        );
        await _mediator.Publish(userNotification, cancel);
    }

    private async Task LogOfficeEventAsync(Office office, bool isActive, int officeId, CancellationToken cancel)
    {
        var action = isActive ? "Office Activated" : "Office Deactivated";
        var message = $"OfficeId: {officeId} {action} by UserId: {_currentUser.UserId} at {DateTime.UtcNow:O}";
        var notification = new IdentityEventLogNotification(
            action,
            _currentUser.Source,
            message,
            _currentUser.UserId,
            office.Id
        );
        await _mediator.Publish(notification, cancel);
    }
}
#pragma warning restore CA2201 // Do not raise reserved exception types
