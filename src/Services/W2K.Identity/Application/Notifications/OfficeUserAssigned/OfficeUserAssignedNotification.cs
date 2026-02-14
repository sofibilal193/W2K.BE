#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Notification to that indicates that a user is assigned to one or more offices.
/// This should be done through a separate notification/handler to maintain cleaner code separation for sending emails etc.
/// </summary>
/// <param name="UserId">The ID of the user being assigned to offices</param>
/// <param name="OfficeIds">The IDs of the offices being assigned</param>
public record OfficeUserAssignedNotification(
    int UserId,
    IReadOnlyList<int> OfficeIds
) : INotification;
