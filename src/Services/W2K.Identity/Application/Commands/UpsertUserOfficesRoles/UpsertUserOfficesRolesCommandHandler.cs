using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Events;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles updating, creating and deleting office role assignments for a user.
/// </summary>
public class UpsertUserOfficesRolesCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser,
    IMediator mediator)
    : IRequestHandler<UpsertUserOfficesRolesCommand>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(UpsertUserOfficesRolesCommand request, CancellationToken cancellationToken)
    {
        // Get existing office assignments for the user (empty list if none exist)
        var officeUsers = await _data.OfficeUsers.GetAsync(x => x.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(OfficeUser), request.UserId);

        // Track newly assigned offices for email notification
        var newlyAssignedOfficeIds = new List<int>();
        var deletedOfficeIds = new HashSet<int>();

        foreach (var officeRole in request.OfficeRoles)
        {
            var officeUser = officeUsers.FirstOrDefault(x => x.OfficeId == officeRole.OfficeId);

            if (officeUser is null && !officeRole.IsDeleted)
            {
                AddOfficeUser(request.UserId, officeRole);
                newlyAssignedOfficeIds.Add(officeRole.OfficeId);
            }
            else if (officeUser is not null && officeRole.IsDeleted)
            {
                DeleteOfficeUser(officeUser);
                deletedOfficeIds.Add(officeRole.OfficeId);
            }
            else if (officeUser is not null)
            {
                UpdateOfficeUser(officeUser, officeRole);
            }
        }

        // Ensure business rule: exactly one active default office must exist if any active offices exist
        EnsureDefaultOfficeInvariant(officeUsers, deletedOfficeIds);

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        // Send email notification if SuperAdmin assigned new offices to merchant user.
        // This should be done through a separate notification/handler to maintain cleaner code separation for sending emails etc.
        if (_currentUser.OfficeType == OfficeType.SuperAdmin && newlyAssignedOfficeIds.Count != 0)
        {
            // Publish single notification with all newly assigned offices for efficient batch processing
            await _mediator.Publish(
                new OfficeUserAssignedNotification(request.UserId, newlyAssignedOfficeIds.AsReadOnly()),
                cancellationToken);
        }
    }

    private void AddOfficeUser(int userId, UserOfficeRoleCommand officeRole)
    {
        var officeUser = new OfficeUser(userId, officeRole.RoleId, officeRole.OfficeId, officeRole.Title);
        officeUser.Invite();
        _data.OfficeUsers.Add(officeUser);
    }

    private void DeleteOfficeUser(OfficeUser officeUser)
    {
        officeUser.Delete();
        _data.OfficeUsers.Remove(officeUser);
    }

    private static void UpdateOfficeUser(OfficeUser officeUser, UserOfficeRoleCommand officeRole)
    {
        officeUser.Update(officeUser.OfficeId, officeRole.RoleId, officeRole.IsDisabled);
    }

    /// <summary>
    /// Enforces the business rule: exactly one active default office must exist if any active offices exist.
    /// User must have at least one office (even if disabled) to prevent complete removal.
    /// </summary>
    private static void EnsureDefaultOfficeInvariant(List<OfficeUser> allOfficeUsers, HashSet<int> deletedOfficeIds)
    {
        // Get all remaining offices (including disabled) after excluding deleted ones
        var remainingOffices = allOfficeUsers
            .Where(x => !deletedOfficeIds.Contains(x.OfficeId))
            .ToList();

        // Business rule: User must have at least one office (even if disabled)
        if (remainingOffices.Count == 0)
        {
            throw new DomainException("User must be associated with at least one Office.");
        }

        // Get active offices for default office assignment
        var activeOffices = remainingOffices
            .Where(x => !x.IsDisabled)
            .OrderBy(x => x.CreateDateTimeUtc)
            .ToList();

        // Clear default flags from offices that are disabled OR being deleted
        foreach (var office in allOfficeUsers.Where(x => x.IsDefault && x.IsDisabled))
        {
            office.ClearIsDefault();
        }

        // If there are active offices, ensure one is set as default
        if (activeOffices.Count > 0)
        {
            // Check if there's an active default office
            var hasActiveDefault = activeOffices.Any(x => x.IsDefault);

            // If no active default exists, set the oldest active office as default
            if (!hasActiveDefault)
            {
                activeOffices[0].SetIsDefault();
            }
        }
    }
}

