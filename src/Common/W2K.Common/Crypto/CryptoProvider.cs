using System.Security.Cryptography;
using System.Text;
using W2K.Common.Utils;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Crypto;

/// <summary>
/// An implementation of ICryptoProvider
/// </summary>
public class CryptoProvider : ICryptoProvider
{
    #region Constants

    private const int _defaultAsymmetricKeySize = 2048;
    private const int _minPasswordHashIterations = 120_000;

    #endregion

    #region Private Properties

    private readonly CryptoOptions _options;
    private readonly ILogger<CryptoProvider> _logger;

    #endregion

    #region Constructors

    public CryptoProvider(CryptoOptions? options, ILogger<CryptoProvider> logger)
    {
        _options = options ?? new CryptoOptions(GenerateKey());
        _logger = logger;
    }

    #endregion

    #region ICryptoProvider Methods

    #region Encryption Functions

    public byte[] GenerateKey()
    {
        byte[]? key = null;
        using (var aesAlg = Aes.Create())
        {
            aesAlg.KeySize = _options?.KeySize ?? 256;
            aesAlg.GenerateKey();
            key = aesAlg.Key;
        }
        return key;
    }

    public CryptoKeys GenerateEncryptionIvAndKey()
    {
        using var aesAlg = Aes.Create();
        aesAlg.KeySize = 256;
        aesAlg.GenerateKey();
        aesAlg.GenerateIV();
        return new CryptoKeys(aesAlg.Key, aesAlg.IV);
    }

    public string GenerateKeyString()
    {
        return Convert.ToBase64String(GenerateKey());
    }

    public string? EncryptString(string? plainText, byte[]? key = null)
    {
        if (plainText is null)
        {
            return null;
        }

        using var aesAlg = Aes.Create();
        aesAlg.GenerateIV();
        var iv = aesAlg.IV;

#pragma warning disable CA5401 // Do not use CreateEncryptor with non-default IV
        using var encryptor = aesAlg.CreateEncryptor(key ?? _options.Key, iv);
#pragma warning restore CA5401 // Do not use CreateEncryptor with non-default IV
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encryptedContent = msEncrypt.ToArray();

        var result = new byte[iv.Length + encryptedContent.Length];

        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

        return Convert.ToBase64String(result);
    }

    public string? EncryptString(string? plainText, byte[]? key, byte[]? iv)
    {
        if (plainText is null || key is null || iv is null)
        {
            return null;
        }

        byte[] encrypted;
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.FeedbackSize = 128;

            aesAlg.Key = key;
            aesAlg.IV = iv;

#pragma warning disable CA5401 // Do not use CreateEncryptor with non-default IV
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
#pragma warning restore CA5401 // Do not use CreateEncryptor with non-default IV

            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            encrypted = msEncrypt.ToArray();
        }

        return Base64UrlEncoder.EncodeFromBytes(encrypted);
    }

    public string? DecryptString(string? cipherText, byte[]? key = null)
    {
        if (cipherText is null)
        {
            return null;
        }

        string? result = null;
        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            using var aesAlg = Aes.Create();
            using var decryptor = aesAlg.CreateDecryptor(key ?? _options.Key, iv);

            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            result = srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            string cipherInfo = $"len={cipherText!.Length}, sha256={Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(cipherText)))}";
            _logger.LogError(ex, "Error occurred while decrypting value (CipherInfo: {CipherInfo}).", cipherInfo);
        }

        return result;
    }

    public string? DecryptString(string? cipherText, byte[]? key, byte[]? iv)
    {
        if (cipherText is null || key is null || iv is null)
        {
            return null;
        }

        var decodedBytes = Base64UrlEncoder.DecodeToBytes(cipherText);

        string? plaintext = null;

        using (var rijAlg = Aes.Create())
        {
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = iv;

            var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            using var msDecrypt = new MemoryStream(decodedBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            plaintext = srDecrypt.ReadToEnd();
        }

        return plaintext;
    }

    #endregion

    #region Hashing Functions

    public string? HashString(string? text)
    {
        string? hashText = null;
        if (!string.IsNullOrEmpty(text))
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            if (hash is not null)
            {
                hashText = Convert.ToBase64String(hash);
            }
        }
        return hashText;
    }

    /// <summary>
    /// Computes a SHA256 hash of the input string and returns it as a short HEX string.
    /// </summary>
    /// <param name="text">The text to hash.</param>
    /// <returns>Short HEX-encoded hash, or null if input is null or empty.</returns>
    public string? ShortHash(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        try
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(bytes, 0, 8);
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Password Hashing Functions

    public string GenerateRandomCode(int min, int max)
    {
        return NumberUtil.GenerateRandomNumber(min, max).ToString();
    }

    ///<Summary>
    /// Returns a base64 encoded hashed version of an input plain text password.
    /// Enforces a minimum iteration count defined by _minPasswordHashIterations.
    ///</Summary>
    public string HashPassword(string password, int iterations, out string base64EncodedSalt)
    {
        int effectiveIterations = iterations < _minPasswordHashIterations ? _minPasswordHashIterations : iterations;
        if (iterations < _minPasswordHashIterations)
        {
            _logger.LogWarning("Password hash iterations {Iterations} below minimum {Minimum}; using minimum.", iterations, _minPasswordHashIterations);
        }
        var hash = GeneratePasswordHash(password, effectiveIterations, 16, 20, out byte[] salt);
        base64EncodedSalt = Convert.ToBase64String(salt);
        return Convert.ToBase64String(hash);
    }

    ///<Summary>
    /// Validates a password by comparing the hashed and non-hashed versions.
    /// If the provided iterations are below the enforced minimum, the minimum is used (must match how it was hashed).
    ///</Summary>
    public bool ValidatePassword(string password, string hashedPassword, string base64EncodedSalt, int iterations)
    {
        int effectiveIterations = iterations < _minPasswordHashIterations ? _minPasswordHashIterations : iterations;
        if (iterations < _minPasswordHashIterations)
        {
            _logger.LogWarning("Password validation iterations {Iterations} below minimum {Minimum}; using minimum.", iterations, _minPasswordHashIterations);
        }
        var hash = GeneratePasswordHash(password, Convert.FromBase64String(base64EncodedSalt), effectiveIterations, 20);
        return string.Equals(Convert.ToBase64String(hash), hashedPassword, StringComparison.Ordinal);
    }

    /// <summary>
    /// Generates a random password
    /// </summary>
    /// <param name="length">minimum length</param>
    /// <param name="strong">generate a strong password</param>
    /// <returns></returns>
    public string GenerateRandomPassword(short length, bool strong = false)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        // Character sets (exclude visually ambiguous chars like l, O similar to previous implementation)
        const string lowers = "abcdefghijkmnopqrstuvwxyz"; // removed l
        const string uppers = "ABCDEFGHJKLMNOPQRSTUVWXYZ"; // removed O
        const string digits = "0123456789";
        const string specials = "!#$%&'()*+,-./:;<=>?@[\\]_"; // same as previous

        if (!strong)
        {
            var nonStrongPool = lowers + uppers + digits; // exclude specials
            var simple = new char[length];
            for (int i = 0; i < length; i++)
            {
                simple[i] = nonStrongPool[RandomNumberGenerator.GetInt32(nonStrongPool.Length)];
            }
            return new string(simple);
        }

        // Strong: guarantee at least one from each category (when length permits)
        var requiredSets = new[] { lowers, uppers, digits, specials };
        var buffer = new char[length];
        int pos = 0;
        foreach (var set in requiredSets)
        {
            if (pos >= length)
            {
                break; // if length < number of categories
            }
            buffer[pos++] = set[RandomNumberGenerator.GetInt32(set.Length)];
        }

        var fullPool = string.Concat(requiredSets);
        for (; pos < length; pos++)
        {
            buffer[pos] = fullPool[RandomNumberGenerator.GetInt32(fullPool.Length)];
        }

        // Shuffle to avoid predictable category ordering
        Shuffle(buffer);
        return new string(buffer);
    }

    #endregion

    #region Asymmetric Encryption Functions

    /// <summary>
    /// Generates a new RSA key pair for asymmetric encryption.
    /// </summary>
    /// <returns>An <see cref="AsymmetricKeyPair"/> containing base64-encoded public and private keys.</returns>
    public AsymmetricKeyPair GenerateAsymmetricKeyPair()
    {
        using var rsa = RSA.Create(_defaultAsymmetricKeySize);
        var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
        var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        return new AsymmetricKeyPair(publicKey, privateKey);
    }

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
    public string? EncryptAsymmetric(string plainText, string publicKeyBase64Encoded)
    {
        if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(publicKeyBase64Encoded))
        {
            return null;
        }
        try
        {
            // Generate AES key and IV
            using var aes = Aes.Create();
            aes.KeySize = _options?.KeySize ?? 256;
            aes.GenerateKey();
            aes.GenerateIV();
            var key = aes.Key;
            var iv = aes.IV;

#pragma warning disable CA5401 // Do not use CreateEncryptor with non-default IV
            // Encrypt data with AES
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, aes.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
            var encryptedData = msEncrypt.ToArray();
#pragma warning restore CA5401 // Do not use CreateEncryptor with non-default IV

            // Encrypt AES key with RSA
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyBase64Encoded), out _);
            var encryptedKey = rsa.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
            var encryptedIv = rsa.Encrypt(iv, RSAEncryptionPadding.OaepSHA256);

            // Combine: [encryptedKeyLength][encryptedKey][encryptedIvLength][encryptedIv][encryptedData]
            var keyLen = BitConverter.GetBytes(encryptedKey.Length);
            var ivLen = BitConverter.GetBytes(encryptedIv.Length);
            using var finalStream = new MemoryStream();
            finalStream.Write(keyLen, 0, keyLen.Length);
            finalStream.Write(encryptedKey, 0, encryptedKey.Length);
            finalStream.Write(ivLen, 0, ivLen.Length);
            finalStream.Write(encryptedIv, 0, encryptedIv.Length);
            finalStream.Write(encryptedData, 0, encryptedData.Length);
            return Convert.ToBase64String(finalStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during hybrid encryption.");
            return null;
        }
    }

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
    public string? DecryptAsymmetric(string cipherText, string privateKeyBase64Encoded)
    {
        if (string.IsNullOrEmpty(cipherText) || string.IsNullOrEmpty(privateKeyBase64Encoded))
        {
            return null;
        }
        try
        {
            var allBytes = Convert.FromBase64String(cipherText);
            int offset = 0;
            var keyLen = BitConverter.ToInt32(allBytes, offset);
            offset += 4;
            var encryptedKey = new byte[keyLen];
            Buffer.BlockCopy(allBytes, offset, encryptedKey, 0, keyLen);
            offset += keyLen;
            var ivLen = BitConverter.ToInt32(allBytes, offset);
            offset += 4;
            var encryptedIv = new byte[ivLen];
            Buffer.BlockCopy(allBytes, offset, encryptedIv, 0, ivLen);
            offset += ivLen;
            var encryptedData = new byte[allBytes.Length - offset];
            Buffer.BlockCopy(allBytes, offset, encryptedData, 0, encryptedData.Length);

            // Decrypt AES key and IV with RSA
            using var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKeyBase64Encoded), out _);
            var key = rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
            var iv = rsa.Decrypt(encryptedIv, RSAEncryptionPadding.OaepSHA256);

            // Decrypt data with AES
            using var aes = Aes.Create();
            using var msDecrypt = new MemoryStream(encryptedData);
            using var csDecrypt = new CryptoStream(msDecrypt, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during hybrid decryption.");
            return null;
        }
    }

    #endregion

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }

    #endregion

    #region Private Methods

    ///<Summary>
    /// Generates a hash for a given text using PBKDF2.
    /// Uses static Pbkdf2 method (Rfc2898DeriveBytes constructor was removed in .NET 10).
    ///</Summary>
    private static byte[] GeneratePasswordHash(string text, int iterations, int saltLength, int outputLength, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(saltLength);
        return Rfc2898DeriveBytes.Pbkdf2(text, salt, iterations, HashAlgorithmName.SHA256, outputLength);
    }

    ///<Summary>
    /// Generates a hash for a given text using PBKDF2.
    /// Uses static Pbkdf2 method (Rfc2898DeriveBytes constructor was removed in .NET 10).
    ///</Summary>
    private static byte[] GeneratePasswordHash(string text, byte[] salt, int iterations, int outputLength)
    {
        return Rfc2898DeriveBytes.Pbkdf2(text, salt, iterations, HashAlgorithmName.SHA256, outputLength);
    }

    /// <summary>
    /// Fisher-Yates shuffle using a cryptographically strong RNG.
    /// </summary>
#pragma warning disable IDE0180 // Use tuple to swap values
    private static void Shuffle(char[] buffer)
    {
        for (int i = buffer.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            // Explicit swap (avoid ValueTuple to satisfy analyzer / deployment environment)
            var tmp = buffer[i];
            buffer[i] = buffer[j];
            buffer[j] = tmp;
        }
    }
#pragma warning restore IDE0180

    #endregion
}
