using DFI.Common.Crypto;
using ProtoBuf;

namespace DFI.Common.Application.DTOs;

[Serializable]
[ProtoContract]
public record OfficeOwnerDto
{
    /// <summary>
    /// The first name of the owner.
    /// </summary>
    [ProtoMember(1)]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// The middle name of the owner (optional).
    /// </summary>
    [ProtoMember(2)]
    public string? MiddleName { get; init; }

    /// <summary>
    /// The last name of the owner.
    /// </summary>
    [ProtoMember(3)]
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// The email address of the owner.
    /// </summary>
    [ProtoMember(4)]
    [JsonEncrypted<string>]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// The mobile phone number of the owner.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string? MobilePhone { get; init; }

    /// <summary>
    /// The Social Security Number (SSN) of the owner.
    /// </summary>
    [ProtoMember(6)]
    [JsonEncrypted<string>]
    public string? SSN { get; init; }

    /// <summary>
    /// The date of birth of the owner.
    /// </summary>
    [ProtoMember(7)]
    [JsonEncrypted<DateOnly>]
    public DateOnly? DOB { get; init; }

    /// <summary>
    /// The ownership percentage of the owner.
    /// </summary>
    [ProtoMember(8)]
    public short Ownership { get; init; }

    /// <summary>
    /// The address of the owner.
    /// </summary>
    [ProtoMember(9)]
    public AddressDto Address { get; init; } = new();
}
