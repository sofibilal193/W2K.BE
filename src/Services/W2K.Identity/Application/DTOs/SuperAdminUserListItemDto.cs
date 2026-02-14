using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.Crypto;
using W2K.Identity.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record SuperAdminUserListItemDto : IMap
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// Email address of the user (encrypted).
    /// </summary>
    [ProtoMember(2)]
    [JsonEncrypted<string>]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// First name of the user
    /// </summary>
    [ProtoMember(3)]
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [ProtoMember(4)]
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Mobile phone number of the user (encrypted).
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string? MobilePhone { get; init; }

    /// <summary>
    /// Indicates if the user is active (not disabled).
    /// </summary>
    [ProtoMember(6)]
    public bool IsActive { get; init; }

    /// <summary>
    /// Date and time of the user's last login (UTC).
    /// </summary>
    [ProtoMember(7)]
    public DateTime LastLoginDateTimeUtc { get; init; }

    /// <summary>
    /// Indicates whether the user has been invited to the system.
    /// </summary>
    [ProtoMember(8)]
    public string Status { get; init; } = String.Empty;

    /// <summary>
    /// Role ID assigned to the SuperAdmin user.
    /// </summary>
    [ProtoMember(9)]
    public int? RoleId { get; init; }

    /// <summary>
    /// Title of the SuperAdmin user.
    /// </summary>
    [ProtoMember(10)]
    public string? Title { get; init; }

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<User, SuperAdminUserListItemDto>()
            .ForMember(x => x.IsActive, x => x.MapFrom(x => !x.IsDisabled && x.DefaultOfficeId > 0))
            .ForMember(x => x.FirstName, x => x.MapFrom(src => src.FirstName ?? string.Empty))
            .ForMember(x => x.LastName, x => x.MapFrom(src => src.LastName ?? string.Empty))
            .ForMember(x => x.LastLoginDateTimeUtc, x => x.MapFrom(src => src.LastLoginDateTimeUtc ?? DateTime.MinValue))
            .ForMember(x => x.Status, x => x.MapFrom(src => UserMappings.MapUserStatus(src)))
            .ForMember(x => x.RoleId, x => x.MapFrom(src => MapOfficeProperty(src, o => o.RoleId)))
            .ForMember(x => x.Title, x => x.MapFrom(src => MapOfficeProperty(src, o => o.Title)));
    }

    private static T? MapOfficeProperty<T>(User src, Func<OfficeUser, T> selector)
    {
        var office = src.Offices.FirstOrDefault();
        return office is null ? default : selector(office);
    }

}
