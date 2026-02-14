namespace W2K.Common.Infrastructure.Settings;

public record AzureADSettings
{
    public string TenantId { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Azure AD B2C tenant domain (e.g., contoso.onmicrosoft.com) used as the issuer for local accounts.
    /// </summary>
    public string Domain { get; init; } = string.Empty;
}
