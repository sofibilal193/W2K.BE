using W2K.Common.Crypto;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record OfficeUserDto
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    [ProtoMember(1)]
    public int UserId { get; init; }

    /// <summary>
    /// Id of the office.
    /// </summary>
    [ProtoMember(2)]
    public int OfficeId { get; init; }

    /// <summary>
    /// Name of the office.
    /// </summary>
    [ProtoMember(3)]
    public string? OfficeName { get; init; }

    /// <summary>
    /// First name of the office user.
    /// </summary>
    [ProtoMember(4)]
    public string? FirstName { get; init; }

    /// <summary>
    /// Last name of the office user.
    /// </summary>
    [ProtoMember(5)]
    public string? LastName { get; init; }

    /// <summary>
    /// Email address of the office user.
    /// </summary>
    [ProtoMember(6)]
    [JsonEncrypted<string>]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Mobile phone number of the office user.
    /// </summary>
    [ProtoMember(7)]
    [JsonEncrypted<string>]
    public string? MobilePhone { get; init; }

    /// <summary>
    /// Title or designation of the office user.
    /// </summary>
    [ProtoMember(8)]
    public string? Title { get; init; }

    /// <summary>
    /// Role of the office user.
    /// </summary>
    [ProtoMember(9)]
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Last login date and time (in UTC) of the office user.
    /// </summary>
    [ProtoMember(10)]
    public DateTime? LastLoginDateTimeUtc { get; init; }

    /// <summary>
    /// A value indicating whether the office user is active.
    /// </summary>
    [ProtoMember(11)]
    public bool IsActive { get; init; } = false;

    /// <summary>
    /// Indicates the default office for the user.
    /// </summary>
    [ProtoMember(12)]
    public bool? IsDefault { get; init; } = false;

    /// <summary>
    /// Date and time (in UTC) when the user invitation was accepted.
    /// </summary>
    [ProtoMember(13)]
    public DateTime? InviteAcceptedDateTimeUtc { get; init; }

    /// <summary>
    /// Enrollment status of the office.
    /// Possible values: Incomplete, Enrolled, Reviewed, Approved, Denied, Inactive
    /// </summary>
    [ProtoMember(14)]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Creates a cleansed copy of the DTO with specific fields set to null to exclude them from JSON serialization.
    /// This is used for SuperAdmin users to exclude sensitive or unnecessary fields from the response.
    /// Fields cleansed: OfficeName, LastLoginDateTimeUtc, InviteAcceptedDateTimeUtc, IsInvited, IsDefault, IsInviteProcessed
    /// </summary>
    public OfficeUserDto Cleanse()
    {
        return this with
        {
            OfficeName = null,
            LastLoginDateTimeUtc = null,
            InviteAcceptedDateTimeUtc = null,
            IsDefault = null,
        };
    }
}
