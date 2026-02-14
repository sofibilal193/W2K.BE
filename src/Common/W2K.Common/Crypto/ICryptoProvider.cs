namespace W2K.Common.Crypto;

public interface ICryptoProvider : IDisposable
{
    #region Encryption Functions

    /// <summary>
    /// Generates a random symmetric encryption key.
    /// </summary>
    byte[] GenerateKey();

    /// <summary>
    /// Generates a random AES key and IV pair.
    /// </summary>
    CryptoKeys GenerateEncryptionIvAndKey();

    /// <summary>
    /// Generates a random symmetric key and returns it as a base64 string.
    /// </summary>
    string GenerateKeyString();

    /// <summary>
    /// Encrypts a string using AES with the provided key. If no key is provided, uses the default key.
    /// </summary>
    /// <param name="plainText">The plaintext to encrypt.</param>
    /// <param name="key">The AES key to use. If null, uses the default key.</param>
    /// <returns>Base64-encoded encrypted string, or null if input is null.</returns>
    string? EncryptString(string? plainText, byte[]? key = null);

    /// <summary>
    /// Encrypts a string using AES with the provided key and IV.
    /// </summary>
    /// <param name="plainText">The plaintext to encrypt.</param>
    /// <param name="key">The AES key to use.</param>
    /// <param name="iv">The AES IV to use.</param>
    /// <returns>Base64-encoded encrypted string, or null if input is null.</returns>
    string? EncryptString(string? plainText, byte[]? key, byte[]? iv);

    /// <summary>
    /// Decrypts a base64-encoded string using AES with the provided key. If no key is provided, uses the default key.
    /// </summary>
    /// <param name="cipherText">The base64-encoded ciphertext to decrypt.</param>
    /// <param name="key">The AES key to use. If null, uses the default key.</param>
    /// <returns>Decrypted plaintext string, or null if input is null or decryption fails.</returns>
    string? DecryptString(string? cipherText, byte[]? key = null);

    /// <summary>
    /// Decrypts a base64-encoded string using AES with the provided key and IV.
    /// </summary>
    /// <param name="cipherText">The base64-encoded ciphertext to decrypt.</param>
    /// <param name="key">The AES key to use.</param>
    /// <param name="iv">The AES IV to use.</param>
    /// <returns>Decrypted plaintext string, or null if input is null or decryption fails.</returns>
    string? DecryptString(string? cipherText, byte[]? key, byte[]? iv);

    #endregion

    #region Hashing Functions

    /// <summary>
    /// Computes a SHA256 hash of the input string and returns it as a base64 string.
    /// </summary>
    /// <param name="text">The text to hash.</param>
    /// <returns>Base64-encoded hash, or null if input is null or empty.</returns>
    string? HashString(string? text);

    /// <summary>
    /// Computes a SHA256 hash of the input string and returns it as a short HEX string.
    /// </summary>
    /// <param name="text">The text to hash.</param>
    /// <returns>Short HEX-encoded hash, or null if input is null or empty.</returns>
    string? ShortHash(string? text);

    #endregion

    #region Password Hashing Functions

    /// <summary>
    /// Generates a random numeric code between min and max (inclusive).
    /// </summary>
    /// <param name="min">Minimum value.</param>
    /// <param name="max">Maximum value.</param>
    /// <returns>Random numeric code as a string.</returns>
    string GenerateRandomCode(int min, int max);

    /// <summary>
    /// Hashes a password using PBKDF2 and returns the hash as a base64 string. Also outputs the salt as a base64 string.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="iterations">Number of hash iterations.</param>
    /// <param name="base64EncodedSalt">Output salt as base64 string.</param>
    /// <returns>Base64-encoded password hash.</returns>
    string HashPassword(string password, int iterations, out string base64EncodedSalt);

    /// <summary>
    /// Validates a password against a hashed password and salt using PBKDF2.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <param name="hashedPassword">The base64-encoded hashed password.</param>
    /// <param name="base64EncodedSalt">The base64-encoded salt.</param>
    /// <param name="iterations">Number of hash iterations.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    bool ValidatePassword(string password, string hashedPassword, string base64EncodedSalt, int iterations);

    /// <summary>
    /// Generates a random password of the specified length. Optionally includes special characters for strong passwords.
    /// </summary>
    /// <param name="length">Length of the password.</param>
    /// <param name="strong">Whether to include special characters.</param>
    /// <returns>Random password string.</returns>
    string GenerateRandomPassword(short length, bool strong = false);

    #endregion

    #region Asymetric Encryption Functions

    /// <summary>
    /// Generates a new RSA key pair for asymmetric encryption.
    /// </summary>
    /// <returns>AsymmetricKeyPair containing base64-encoded public and private keys.</returns>
    AsymmetricKeyPair GenerateAsymmetricKeyPair();

    /// <summary>
    /// Hybrid encryption: encrypts <paramref name="plainText"/> with a random AES key, then encrypts the AES key and IV with the RSA public key.
    /// The returned value is a base64 string containing: [4 bytes key length][encrypted key][4 bytes IV length][encrypted IV][encrypted data].
    /// </summary>
    /// <param name="plainText">The plaintext to encrypt.</param>
    /// <param name="publicKeyBase64Encoded">The RSA public key (base64-encoded).</param>
    /// <returns>
    /// A base64-encoded string containing the encrypted AES key, IV, and encrypted data, or null if input is invalid.
    /// Format: [keyLen][encryptedKey][ivLen][encryptedIv][encryptedData]
    /// </returns>
    string? EncryptAsymmetric(string plainText, string publicKeyBase64Encoded);

    /// <summary>
    /// Hybrid decryption: extracts and decrypts the AES key and IV with the RSA private key, then decrypts the data with AES.
    /// Accepts a base64 string containing: [4 bytes key length][encrypted key][4 bytes IV length][encrypted IV][encrypted data].
    /// </summary>
    /// <param name="cipherText">The base64-encoded encrypted payload.</param>
    /// <param name="privateKeyBase64Encoded">The RSA private key (base64-encoded).</param>
    /// <returns>
    /// The decrypted plaintext string, or null if input is invalid or decryption fails.
    /// Format of <paramref name="cipherText"/>: [keyLen][encryptedKey][ivLen][encryptedIv][encryptedData]
    /// </returns>
    string? DecryptAsymmetric(string cipherText, string privateKeyBase64Encoded);

    #endregion
}
