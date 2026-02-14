using System.Text.Json.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Command to upsert (add/update/delete) a batch of office role assignments for a user.
/// </summary>
public sealed record UpsertUserOfficesRolesCommand : IRequest
{
    [JsonIgnore]
    public int UserId { get; private set; }

    public required IEnumerable<UserOfficeRoleCommand> OfficeRoles { get; init; } = [];

    public void SetUserId(int userId) =>
                                        UserId = userId;
}

/// <summary>
/// Represents a single office/role mutation for a user.
/// </summary>
public readonly record struct UserOfficeRoleCommand
{
    /// <summary>
    /// The ID of the office to which the user is assigned.
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// The ID of the role assigned to the user in the office.
    /// </summary>
    public int RoleId { get; init; }

    /// <summary>
    /// The title or designation of the user in the office.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Indicates whether the office assignment is disabled for the user.
    /// </summary>
    public bool IsDisabled { get; init; }

    /// <summary>
    /// Indicates whether the office assignment is marked for deletion.
    /// </summary>
    public bool IsDeleted { get; init; }
}
