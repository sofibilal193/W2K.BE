using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct LoginUserInfoDto : IMap
{
    [ProtoMember(1)]
    public int Id { get; init; }

    [ProtoMember(2)]
    public string? FirstName { get; init; }

    [ProtoMember(3)]
    public string? LastName { get; init; }

    [ProtoMember(4)]
    public string Email { get; init; } = string.Empty;

    [ProtoMember(5)]
    public string? MobilePhone { get; init; }

    [ProtoMember(6)]
    public int? OfficeId { get; init; }

    [ProtoMember(7)]
    public string? OfficeType { get; init; }

    [ProtoMember(8)]
    public string? OfficeName { get; init; }

    [ProtoMember(9)]
    public string? OfficeRole { get; init; }

    [ProtoMember(10)]
    public string? OfficeTitle { get; init; }

    [ProtoMember(11)]
    public IList<string>? OfficePermissions { get; init; }

    [ProtoMember(12)]
    public bool IsMultipleOffices { get; init; }

    [ProtoMember(13)]
    public IList<int> OfficeIds { get; init; } = [];

    [ProtoMember(14)]
    public bool IsUserDisabled { get; init; }

    [ProtoMember(15)]
    public bool IsOfficeUserDisabled { get; init; }

    [ProtoMember(16)]
    public bool IsSuperAdmin { get; init; }

    [ProtoMember(17)]
    public bool IsEnrollmentCompleted { get; init; }

    public LoginUserInfoDto()
    {
    }

    public void Mapping(Profile profile)
    {
        var map = profile.CreateMap<User, LoginUserInfoDto>();
        MapBasicFields(map);
        MapOfficeFields(map);
        MapOfficePermissions(map);
        MapOfficeType(map);
        MapSuperAdmin(map);
        MapEnrollmentCompleted(map);
    }

    private static void MapBasicFields(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(x => x.Id, x => x.MapFrom(s => s.Id))
            .ForMember(x => x.FirstName, x => x.MapFrom(s => s.FirstName))
            .ForMember(x => x.LastName, x => x.MapFrom(s => s.LastName))
            .ForMember(x => x.Email, x => x.MapFrom(s => s.Email))
            .ForMember(x => x.MobilePhone, x => x.MapFrom(s => s.MobilePhone))
            .ForMember(x => x.IsUserDisabled, x => x.MapFrom(s => s.IsDisabled))
            .ForMember(x => x.IsMultipleOffices, x => x.MapFrom(s => s.Offices != null && s.Offices.Count(o => !o.IsDisabled) > 1))
            .ForMember(x => x.OfficeIds, x => x.MapFrom(s => s.Offices.Where(o => !o.IsDisabled).Select(o => o.OfficeId)));
    }

    private static void MapOfficeFields(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(x => x.OfficeId, x => x.MapFrom(s => s.DefaultOffice == null ? (int?)null : s.DefaultOffice.OfficeId))
            .ForMember(x => x.OfficeName, x => x.MapFrom(s => s.DefaultOffice == null || s.DefaultOffice.Office == null ? null : s.DefaultOffice.Office.Name))
            .ForMember(x => x.OfficeRole, x => x.MapFrom(s => s.DefaultOffice == null || s.DefaultOffice.Role == null ? null : s.DefaultOffice.Role.Name))
            .ForMember(x => x.OfficeTitle, x => x.MapFrom(s => s.DefaultOffice == null ? null : s.DefaultOffice.Title))
            .ForMember(x => x.IsOfficeUserDisabled, x => x.MapFrom(s => s.DefaultOffice != null && s.DefaultOffice.IsDisabled));
    }

    private static void MapOfficePermissions(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(
            x => x.OfficePermissions,
            x => x.MapFrom(
                    s => s.DefaultOffice == null || s.DefaultOffice.Role == null || s.DefaultOffice.Role.Permissions == null
                            ? null
                            : s.DefaultOffice.Role.Permissions.Select(p => p.Name).ToList()));
    }

    private static void MapOfficeType(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(
            x => x.OfficeType,
            x => x.MapFrom(
                    s => s.DefaultOffice == null || s.DefaultOffice.Office == null
                            ? null
                            : s.DefaultOffice.Office.Type.ToString()));
    }

    private static void MapSuperAdmin(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(
            x => x.IsSuperAdmin,
            x => x.MapFrom(
                    s => s.DefaultOffice != null
                            && s.DefaultOffice.Office != null
                            && s.DefaultOffice.Office.Type == Common.Identity.OfficeType.SuperAdmin));
    }

    private static void MapEnrollmentCompleted(IMappingExpression<User, LoginUserInfoDto> map)
    {
        _ = map.ForMember(
            x => x.IsEnrollmentCompleted,
            x => x.MapFrom(
                    s => s.DefaultOffice != null
                            && s.DefaultOffice.Office != null
                            && s.DefaultOffice.Office.IsEnrollmentCompleted));
    }
}
