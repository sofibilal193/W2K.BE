using System.Text.Json.Serialization;
using W2K.Common.Crypto;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpsertSuperAdminUserCommand : IRequest<int?>
{
    /// <summary>
    /// The ID of the user being updated, or null for a new user.
    /// </summary>
    [JsonIgnore]
    public int? UserId { get; private set; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    [JsonEncrypted<string>]
    public required string Email { get; init; }

    /// <summary>
    /// The mobile phone number of the user.
    /// </summary>
    [JsonEncrypted<string>]
    public required string MobilePhone { get; init; }

    /// <summary>
    /// The job title of the user.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// The ID of the role assigned to the user.
    /// Required for SuperAdmin users.
    /// </summary>
    public required int RoleId { get; init; }

    /// <summary>
    /// Sets the user ID for the command.
    /// </summary>
    /// <param name="userId">The user ID, or null for a new user.</param>
    public void SetUserId(int? userId)
    {
        UserId = userId;
    }
}
