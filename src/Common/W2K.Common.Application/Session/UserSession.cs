using ProtoBuf;

namespace W2K.Common.Application.Session;

/// <summary>
/// Represents a unique user session.
/// This model is stored in Cache and is used to track user sessions.
/// </summary>
[Serializable]
[ProtoContract]
public readonly record struct UserSession
{
    /// <summary>
    /// Unique session identifier.
    /// </summary>
    [ProtoMember(1)]
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// User identifier for the session.
    /// This will be the Azure AD Object ID of the user.
    /// If the session is not associated with an authenticated user, this will be null.
    /// This is NOT the internal numeric UserId in our database.
    /// </summary>
    [ProtoMember(2)]
    public string? UserId { get; init; }

    /// <summary>
    /// Fingerprint of the session, base64 encoded.
    /// </summary>
    [ProtoMember(3)]
    public string Base64FingerPrint { get; init; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the session was issued.
    /// </summary>
    [ProtoMember(4)]
    public DateTime IssuedAtUtc { get; init; }

    /// <summary>
    /// UTC timestamp when the session expires.
    /// </summary>
    [ProtoMember(5)]
    public DateTime ExpiresAtUtc { get; init; }

    /// <summary>
    /// Client encryption key, base64 encoded.
    /// Used by the API to encrypt sensitive data in the response payload in the session returned to the client.
    /// This key will be used to encrypt the AES Key encryptor and IV that is used to actually encrypt the data.
    /// </summary>
    [ProtoMember(6)]
    public string? ClientEncryptionKeyBase64Encoded { get; init; }

    /// <summary>
    /// Server encryption key, base64 encoded.
    /// This is used by the client to encrypt sensitive data in the payload before sending it to the server.
    /// This key will be used to encrypt the AES Key encryptor and IV that is used to actually encrypt the data.
    /// </summary>
    [ProtoMember(7)]
    public string? ServerEncryptionKeyBase64Encoded { get; init; }

    /// <summary>
    /// Server decryption key, base64 encoded.
    /// This is used by the API to decrypt sensitive data in the payload before processing it.
    /// This key will be used to decrypt the AES Key encryptor and IV that is used to actually decrypt the encrypted data.
    /// </summary>
    [ProtoMember(8)]
    public string? ServerDecryptionKeyBase64Encoded { get; init; }

    /// <summary>
    /// Indicates whether the session is expired.
    /// </summary>
    public bool IsExpired => ExpiresAtUtc < DateTime.UtcNow;

    /// <summary>
    /// Gets the remaining minutes until session expiration.
    /// </summary>
    public double RemainingMinutes => (ExpiresAtUtc - DateTime.UtcNow).TotalMinutes;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSession"/> struct.
    /// </summary>
    public UserSession()
    {
    }

    /// <summary>
    /// Indicates whether the session needs refresh based on the threshold.
    /// </summary>
    /// <param name="thresholdMinutes">The threshold in minutes to determine if refresh is needed.</param>
    /// <returns>True if remaining minutes is less than or equal to threshold and session is not expired.</returns>
    public bool NeedsRefresh(int thresholdMinutes)
    {
        return !IsExpired && RemainingMinutes <= thresholdMinutes;
    }
}
