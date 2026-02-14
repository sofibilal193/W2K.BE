using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;

namespace W2K.Common.Infrastructure.Crypto;

public class SecretRepository(SecretClient secretClient, KeyClient keyClient) : ISecretRepository
{
    private readonly string _wrapAlgorithm = "RSA-OAEP-256";
    private readonly SecretClient _secretClient = secretClient;
    private readonly KeyClient _keyClient = keyClient;

    #region Constructors

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region ISecretRepository Methods

    #region Secrets

    public async Task<string> GetSecretValueAsync(
        string name,
        CancellationToken cancel = default)
    {
        var secret = await _secretClient.GetSecretAsync(name, cancellationToken: cancel);
        return secret?.Value?.Value ?? string.Empty;
    }

    public async Task<string> UpsertSecretAsync(
        string name,
        string value,
        CancellationToken cancel = default)
    {
        _ = await _secretClient.SetSecretAsync(name, value, cancel);
        return new Uri(_secretClient.VaultUri, "secrets/" + name).ToString();
    }

    public async Task DeleteSecretAsync(
        string name,
        CancellationToken cancel = default)
    {
        var op = await _secretClient.StartDeleteSecretAsync(name, cancel);
        while (!op.HasCompleted)
        {
            await Task.Delay(2000, cancel);
            _ = await op.UpdateStatusAsync(cancel);
        }
        var secret = op.Value;
        _ = await _secretClient.PurgeDeletedSecretAsync(secret.Name, cancel);
    }

    #endregion

    #region Keys

    public async Task<string> WrapKeyAsync(
        byte[] key,
        string keyName,
        CancellationToken cancel = default)
    {
        var value = string.Empty;
        var result = await _keyClient.GetCryptographyClient(keyName)
            .WrapKeyAsync(
                _wrapAlgorithm,
                key,
                cancel);
        if (result?.EncryptedKey is not null)
        {
            value = Convert.ToBase64String(result.EncryptedKey);
        }
        return value;
    }

    public async Task<byte[]> UnWrapKeyAsync(
        string base64EncodedWrappedKey,
        string keyName,
        CancellationToken cancel = default)
    {
        var result = await _keyClient.GetCryptographyClient(keyName)
            .UnwrapKeyAsync(
                _wrapAlgorithm,
                Convert.FromBase64String(base64EncodedWrappedKey),
                cancel);
        return result?.Key ?? [];
    }

    #endregion

    #endregion

    #region Protected/Private Methods

    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }

    #endregion
}
