namespace W2K.Common.Infrastructure.Settings;

public record CryptoSettings
{
    #region Crypto Settings

    public string WrappedEncryptionKeyBase64Encoded { get; init; } = string.Empty;

    public string EncryptionWrapKeyName { get; init; } = string.Empty;

    public string? ClientEncryptionKeyBase64Encoded { get; init; }

    public string? ClientEncryptionIVBase64Encoded { get; init; }

    #endregion

    #region Azure Key Vault Settings

    public string AzureKeyVaultUri { get; init; } = string.Empty;

    #endregion
}
