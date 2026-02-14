namespace W2K.Common.Infrastructure.Crypto;

public interface ISecretRepository : IDisposable
{
    #region Secrets

    Task<string> UpsertSecretAsync(
        string name,
        string value,
        CancellationToken cancel = default
    );

    Task<string> GetSecretValueAsync(
        string name,
        CancellationToken cancel = default);

    Task DeleteSecretAsync(
        string name,
        CancellationToken cancel = default);

    #endregion

    #region Keys

    Task<string> WrapKeyAsync(
        byte[] key,
        string keyName,
        CancellationToken cancel = default);

    Task<byte[]> UnWrapKeyAsync(
        string base64EncodedWrappedKey,
        string keyName,
        CancellationToken cancel = default);

    #endregion
}
