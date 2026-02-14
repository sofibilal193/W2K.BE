
using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record UserOfficeRoleDto : IMap
{
    /// <summary>
    /// Id of the office.
    /// </summary>
    [ProtoMember(1)]
    public int OfficeId { get; init; }

    /// <summary>
    /// Name of the office.
    /// </summary>
    [ProtoMember(2)]
    public string OfficeName { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if the office is disabled for the user.
    /// </summary>
    [ProtoMember(3)]
    public bool IsDisabled { get; init; }

    /// <summary>
    /// Id of the role.
    /// </summary>
    [ProtoMember(4)]
    public int RoleId { get; init; }

    /// <summary>
    /// Title of the user in the office.
    /// </summary>
    [ProtoMember(5)]
    public string Title { get; init; } = string.Empty;

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<OfficeUser, UserOfficeRoleDto>()
            .ForMember(x => x.OfficeName, x => x.MapFrom(src => MapOfficeName(src)));
    }

    private static string MapOfficeName(OfficeUser src)
    {
        return src.Office is null ? string.Empty : src.Office.Name;
    }
}
