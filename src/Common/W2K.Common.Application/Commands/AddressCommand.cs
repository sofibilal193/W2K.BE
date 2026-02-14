using W2K.Common.Crypto;

namespace W2K.Common.Application.Commands;

public record AddressCommand
{
    public string Type { get; init; } = string.Empty;

    [JsonEncrypted<string>]
    public string Address1 { get; init; } = string.Empty;

    [JsonEncrypted<string>]
    public string? Address2 { get; init; }

    [JsonEncrypted<string>]
    public string City { get; init; } = string.Empty;

    public string? County { get; init; }

    [JsonEncrypted<string>]
    public string State { get; init; } = string.Empty;

    [JsonEncrypted<string>]
    public string ZipCode { get; init; } = string.Empty;

    public string Country { get; init; } = "USA";

    public string? TimeZone { get; init; }

    public string? GooglePlaceId { get; init; }
}
