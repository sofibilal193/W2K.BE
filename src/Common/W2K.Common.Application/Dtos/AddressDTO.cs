using W2K.Common.Crypto;
using ProtoBuf;

namespace W2K.Common.Application.DTOs;

[Serializable]
[ProtoContract]
public record AddressDto
{
    /// <summary>
    /// The Type of the address.
    /// </summary>
    [ProtoMember(1)]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The first line of the street address.
    /// </summary>
    [ProtoMember(2)]
    [JsonEncrypted<string>]
    public string Address1 { get; init; } = string.Empty;

    /// <summary>
    /// The second line of the street address.
    /// </summary>
    [ProtoMember(3)]
    [JsonEncrypted<string>]
    public string? Address2 { get; init; }

    /// <summary>
    /// The city of the address.
    /// </summary>
    [ProtoMember(4)]
    [JsonEncrypted<string>]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// The state of the address.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// The postal code of the address.
    /// </summary>
    [ProtoMember(6)]
    [JsonEncrypted<string>]
    public string ZipCode { get; init; } = string.Empty;

    /// <summary>
    /// The country of the address.
    /// </summary>
    [ProtoMember(7)]
    public string Country { get; init; } = "USA"; // Default to USA

    /// <summary>
    /// The county of the address.
    /// </summary>
    [ProtoMember(8)]
    public string? County { get; init; }
}
