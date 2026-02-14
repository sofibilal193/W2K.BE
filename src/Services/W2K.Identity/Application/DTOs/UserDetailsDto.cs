using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.Crypto;
using W2K.Identity.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

/// <summary>
/// Contains detailed information for a user including what offices the user is associated with.
/// </summary>
[Serializable]
[ProtoContract]
public record UserDetailsDto : IMap
{
    /// <summary>
    /// Id of the user.
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
    /// Date and time (UTC) when the user account was created.
    /// </summary>
    [ProtoMember(5)]
    public DateTime? CreatedDateTimeUtc { get; init; }

    /// <summary>
    /// Date and time (UTC) when the user account was last updated.
    /// </summary>
    [ProtoMember(6)]
    public DateTime? LastUpdatedDateTimeUtc { get; init; }

    /// <summary>
    /// Date and time (UTC) when the user last logged in.
    /// </summary>
    [ProtoMember(7)]
    public DateTime? LastLoginDateTimeUtc { get; init; }

    /// <summary>
    /// Indicates whether the user account is active (not disabled).
    /// </summary>
    [ProtoMember(8)]
    public bool IsActive { get; init; }

    /// <summary>
    /// The mobile phone number of the user.
    /// </summary>
    [ProtoMember(9)]
    [JsonEncrypted<string>]
    public string? MobilePhone { get; init; }

    /// <summary>
    /// The status of the user.
    /// </summary>
    [ProtoMember(10)]
    public string? Status { get; init; }

    /// <summary>
    /// The last login IP address of the user.
    /// </summary>
    [ProtoMember(11)]
    [JsonEncrypted<string>]
    public string? LastLoginIpAddress { get; init; }

    /// <summary>
    /// Collection of office associations for this user, including office name, title, role, and enabled status.
    /// </summary>
    [ProtoMember(12)]
    public IReadOnlyList<UserOfficeRoleDto> Offices { get; init; } = [];

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<User, UserDetailsDto>()
            .ForMember(x => x.CreatedDateTimeUtc, x => x.MapFrom(src => src.CreateDateTimeUtc))
            .ForMember(x => x.LastUpdatedDateTimeUtc, x => x.MapFrom(src => src.ModifyDateTimeUtc))
            .ForMember(x => x.LastLoginDateTimeUtc, x => x.MapFrom(src => src.LastLoginDateTimeUtc))
            .ForMember(x => x.IsActive, x => x.MapFrom(src => !src.IsDisabled))
            .ForMember(x => x.MobilePhone, x => x.MapFrom(src => src.MobilePhone))
            .ForMember(x => x.Status, x => x.MapFrom(src => UserMappings.MapUserStatus(src)))
            .ForMember(x => x.Offices, x => x.MapFrom(src => src.Offices ?? Array.Empty<OfficeUser>()))
            .ForMember(x => x.LastLoginIpAddress, x => x.MapFrom(src => src.CreateSource));
    }

}
