using System.Text.Json.Serialization;

namespace W2K.Identity.Controllers.Auth;

public record AzureAdB2CRequest
{
    [JsonPropertyName("objectId")]
    public string? Id { get; init; }

    [JsonPropertyName("email")]
    public string Email { get; init; } = "";

    [JsonPropertyName("givenName")]
    public string? FirstName { get; init; }

    [JsonPropertyName("surname")]
    public string? LastName { get; init; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("extension_MobilePhone")]
    public string? MobilePhone { get; init; }

    [JsonPropertyName("step")]
    public string Step { get; init; } = "SignIn";

    [JsonPropertyName("extension_IpAddress")]
    public string? IpAddress { get; init; }
}
