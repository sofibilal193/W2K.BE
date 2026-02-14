using AutoMapper;
using W2K.Identity.Entities;
using ProtoBuf;
using W2K.Common.Application.Mappings;
using W2K.Common.Crypto;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record UserDetailDto : IMap
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [ProtoMember(2)]
    [JsonEncrypted<string>]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// First name of the user.
    /// </summary>
    [ProtoMember(3)]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [ProtoMember(4)]
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Mobile phone number of the user.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string MobilePhone { get; init; } = string.Empty;

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<User, UserDetailDto>();
    }
}
