// https://docs.microsoft.com/en-us/azure/active-directory-b2c/add-api-connector-token-enrichment?pivots=b2c-user-flow

using System.Text.Json.Serialization;

namespace W2K.Identity.Controllers.Auth;

public record ResponseContent
{
    public static readonly string ApiVersion = "1.0.0";

    [JsonPropertyName("version")]
    public string Version { get; }

    [JsonPropertyName("action")]
    public string Action { get; init; } = "";

    [JsonPropertyName("givenName")]
    public string? FirstName { get; init; }

    [JsonPropertyName("surname")]
    public string? LastName { get; init; }

    [JsonPropertyName("displayName")]
    public string? FullName => string.Concat(FirstName, " ", LastName);

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("extension_UserId")]
    public int? Id { get; init; }

    [JsonPropertyName("extension_MobilePhone")]
    public string? MobilePhone { get; init; }

    [JsonPropertyName("extension_OfficeId")]
    public int? OfficeId { get; init; }

    [JsonPropertyName("extension_OfficeType")]
    public string? OfficeType { get; init; }

    [JsonPropertyName("extension_OfficeName")]
    public string? OfficeName { get; init; }

    [JsonPropertyName("extension_Role")]
    public string? OfficeRole { get; init; }

    [JsonPropertyName("extension_OfficePermissions")]
    public string? OfficePermissions { get; init; }

    [JsonPropertyName("jobTitle")]
    public string? OfficeTitle { get; init; }

    [JsonPropertyName("extension_IsMultipleOffices")]
    public bool? IsMultipleOffices { get; init; }

    [JsonPropertyName("extension_OfficeIds")]
    public string? OfficeIds { get; init; }

    [JsonPropertyName("extension_IsDisabled")]
    public bool? IsUserDisabled { get; init; }

    [JsonPropertyName("extension_IsOfficeUserDisabled")]
    public bool? IsOfficeUserDisabled { get; init; }

    [JsonPropertyName("extension_IsSuperAdmin")]
    public bool IsSuperAdmin { get; init; }

    [JsonPropertyName("extension_IsEnrollmentCompleted")]
    public bool IsEnrollmentCompleted { get; init; }

    public ResponseContent()
    {
        Version = ApiVersion;
        Action = "Continue";
    }
}
