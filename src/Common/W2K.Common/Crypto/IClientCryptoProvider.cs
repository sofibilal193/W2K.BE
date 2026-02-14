namespace W2K.Common.Crypto;

public interface IClientCryptoProvider
{
    /// <summary>
    /// Encrypts a string using the client's public encryption key.
    /// </summary>
    /// <param name="value">Value to encrypt.</param>
    /// <returns>Base-64 encoded encrypted string.</returns>
    string? EncryptString(string? value);

    /// <summary>
    /// Decrypts a string using the server's private decryption key.
    /// </summary>
    /// <param name="value">Base-64 encoded encrypted string to decrypt.</param>
    /// <returns>Decrypted string.</returns>
    string? DecryptString(string? value);
}
