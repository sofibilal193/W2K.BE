using System.Text.Json.Serialization;
using W2K.Common.Crypto;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpsertOfficeUserCommand : IRequest<int?>
{
    /// <summary>
    /// The ID of the office to which the user belongs.
    /// </summary>
    [JsonIgnore]
    public int? OfficeId { get; private set; }

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
    /// Required when OfficeId is provided.
    /// </summary>
    public int? RoleId { get; init; }

    /// <summary>
    /// Indicates whether this office is the user's default office.
    /// </summary>
    public required bool IsDefaultOffice { get; init; }

    /// <summary>
    /// If true, send email notification to user after update; if false, do not send email.
    /// </summary>
    public required bool SendUserNotification { get; init; }

    /// <summary>
    /// Sets the office and user IDs for the command.
    /// </summary>
    /// <param name="officeId">The office ID.</param>
    /// <param name="userId">The user ID, or null for a new user.</param>
    public void SetIds(int? officeId, int? userId)
    {
        OfficeId = officeId;
        UserId = userId;
    }
}
