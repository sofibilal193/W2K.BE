#pragma warning disable IDE0046 // Use conditional expression for return

using W2K.Common.Application.Auth;
using W2K.Common.Application.Identity;
using W2K.Common.Application.Session;
using W2K.Common.Application.Settings;
using W2K.Common.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace W2K.Common.Application.Crypto;

public class ClientCryptoProvider(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AppSettings> settings,
    ICryptoProvider cryptoProvider,
    ISessionStore sessionStore) : IClientCryptoProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ICryptoProvider _cryptoProvider = cryptoProvider;
    private readonly ISessionStore _sessionStore = sessionStore;
    private readonly AppSettings _settings = settings.Value;

    /// <summary>
    /// Encrypts a string using the user's session encryption key.
    /// Finds the session by <see cref="AuthConstants.SessionIdHeaderName"/> header.
    /// </summary>
    /// <param name="value">Text to encrypt</param>
    /// <returns>Encrypted text, or null if encryption fails</returns>
    public string? EncryptString(string? value)
    {
        if (!string.IsNullOrEmpty(value) && !IsInternalServiceCommunication())
        {
            try
            {
                if (!_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
                {
                    return value;
                }

                var sessionId = GetSessionId();
                if (sessionId is null)
                {
                    return null;
                }

                var session = _sessionStore.Get(sessionId);
                if (session is null || session.Value.IsExpired || string.IsNullOrEmpty(session.Value.ClientEncryptionKeyBase64Encoded))
                {
                    return null;
                }

                return _cryptoProvider.EncryptAsymmetric(value, session.Value.ClientEncryptionKeyBase64Encoded);
            }
            catch (Exception)
            {
                return null;
            }
        }
        return value;
    }

    /// <summary>
    /// Decrypts a string using the user's session decryption key.
    /// Finds the session by <see cref="AuthConstants.SessionIdHeaderName"/> header.
    /// </summary>
    /// <param name="value">Encrypted text to decrypt</param>
    /// <returns>Decrypted text, or null if decryption fails</returns>
    public string? DecryptString(string? value)
    {
        if (!_settings.AuthSettings.SessionSettings.EnforceOnAllRequests)
        {
            return value;
        }

        if (string.IsNullOrEmpty(value) || IsInternalServiceCommunication())
        {
            return value;
        }

        if (!IsBase64String(value))
        {
            return value;
        }

        try
        {
            var sessionId = GetSessionId();
            if (sessionId is null)
            {
                return null;
            }

            var session = _sessionStore.Get(sessionId);
            if (session is null || session.Value.IsExpired || string.IsNullOrEmpty(session.Value.ServerDecryptionKeyBase64Encoded))
            {
                return null;
            }

            return _cryptoProvider.DecryptAsymmetric(value, session.Value.ServerDecryptionKeyBase64Encoded);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private string? GetSessionId()
    {
        if (_httpContextAccessor.HttpContext?.Request is null)
        {
            return null;
        }
        return _httpContextAccessor.HttpContext.Request.Headers
            .TryGetValue(AuthConstants.SessionIdHeaderName, out var values)
                ? values[0]
                : null;
    }

    private bool IsInternalServiceCommunication()
    {
        if (_httpContextAccessor.HttpContext?.Request is null)
        {
            return false;
        }

        var authKey = _httpContextAccessor.HttpContext.GetHeaderValueAs<string>(AuthConstants.InternalServiceAuthKeyHeaderName);

        if (string.IsNullOrEmpty(authKey) || string.IsNullOrEmpty(_settings.AuthSettings.InternalServiceAuthKey))
        {
            return false;
        }

        return string.Equals(authKey, _settings.AuthSettings.InternalServiceAuthKey, StringComparison.Ordinal);
    }

    private static bool IsBase64String(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length % 4 != 0)
        {
            return false;
        }

        Span<byte> buffer = new byte[value.Length];
        return Convert.TryFromBase64String(value, buffer, out _);
    }
}

#pragma warning restore IDE0046 // Use conditional expression for return
