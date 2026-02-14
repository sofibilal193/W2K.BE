using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record UserInviteInfoDto
{
    /// <summary>
    /// Name of the office.
    /// </summary>
    [ProtoMember(1)]
    public string OfficeName { get; init; } = string.Empty;

    /// <summary>
    /// Street address of office.
    /// </summary>
    [ProtoMember(2)]
    public string Address { get; init; } = string.Empty;

    /// <summary>
    /// City of office.
    /// </summary>
    [ProtoMember(3)]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// State of office.
    /// </summary>
    [ProtoMember(4)]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Zipcode of office.
    /// </summary>
    [ProtoMember(5)]
    public string ZipCode { get; init; } = string.Empty;

    /// <summary>
    /// User phone number (masked - only last 4 digits visible).
    /// </summary>
    [ProtoMember(6)]
    public string? MaskedPhoneNumber { get; init; }
}
