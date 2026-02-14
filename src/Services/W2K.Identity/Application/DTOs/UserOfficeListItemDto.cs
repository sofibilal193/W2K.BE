using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.ValueObjects;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record UserOfficeListItemDto : IMap
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
    /// City where the office is located.
    /// </summary>
    [ProtoMember(3)]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// State or province for the office location.
    /// </summary>
    [ProtoMember(4)]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Indicates whether the office is active (not disabled).
    /// </summary>
    [ProtoMember(5)]
    public bool? IsActive { get; init; }

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<OfficeUser, UserOfficeListItemDto>()
            .ForMember(x => x.OfficeName, x => x.MapFrom(src => MapOfficeName(src)))
            .ForMember(x => x.City, x => x.MapFrom(src => MapCity(src)))
            .ForMember(x => x.State, x => x.MapFrom(src => MapState(src)))
            .ForMember(x => x.IsActive, x => x.MapFrom(src => !src.IsDisabled));
    }

    private static string MapOfficeName(OfficeUser src)
    {
        return src.Office?.Name ?? string.Empty;
    }

    private static string MapCity(OfficeUser src)
    {
        return SelectAddress(src)?.City ?? string.Empty;
    }

    private static string MapState(OfficeUser src)
    {
        return SelectAddress(src)?.State ?? string.Empty;
    }

    private static Address? SelectAddress(OfficeUser src)
    {
        return src.Office?.Addresses?.FirstOrDefault();
    }
}

