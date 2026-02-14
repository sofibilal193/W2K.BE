using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.Application.Session;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct UserSessionDto : IMap
{
    /// <summary>
    /// Unique session identifier.
    /// </summary>
    [ProtoMember(1)]
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the session was issued.
    /// </summary>
    [ProtoMember(2)]
    public DateTime IssuedAtUtc { get; init; }

    /// <summary>
    /// UTC timestamp when the session expires.
    /// </summary>
    [ProtoMember(3)]
    public DateTime ExpiresAtUtc { get; init; }

    /// <summary>
    /// Server encryption key, base64 encoded.
    /// This is used by the client to encrypt sensitive data in the payload before sending it to the server.
    /// This key will be used to encrypt the AES Key encryptor and IV that is used to actually encrypt the data.
    /// </summary>
    [ProtoMember(4)]
    public string? ServerEncryptionKeyBase64Encoded { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSessionDto"/> struct.
    /// </summary>
    public UserSessionDto()
    {
    }

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<UserSession, UserSessionDto>();
    }
}
