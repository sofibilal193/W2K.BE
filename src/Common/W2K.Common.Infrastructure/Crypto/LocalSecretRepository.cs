namespace DFI.Common.Infrastructure.Crypto;

public class LocalSecretRepository : ISecretRepository
{
    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region ISecretRepository Methods

    #region Secrets

    public Task<string> GetSecretValueAsync(
        string name,
        CancellationToken cancel = default)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return Task.FromResult(value ?? string.Empty);
    }

    public Task<string> UpsertSecretAsync(
        string name,
        string value,
        CancellationToken cancel = default)
    {
        Environment.SetEnvironmentVariable(name, value);
        return Task.FromResult(name);
    }

    public Task DeleteSecretAsync(
        string name,
        CancellationToken cancel = default)
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Keys

    public Task<string> WrapKeyAsync(
        byte[] key,
        string keyName,
        CancellationToken cancel = default)
    {
        return Task.FromResult(Convert.ToBase64String(key));
    }

    public Task<byte[]> UnWrapKeyAsync(
        string base64EncodedWrappedKey,
        string keyName,
        CancellationToken cancel = default)
    {
        return Task.FromResult(Convert.FromBase64String(base64EncodedWrappedKey));
    }

    #endregion

    #endregion

    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }
}
