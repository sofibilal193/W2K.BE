using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

/// <summary>
/// DTO representing a search result for offices.
/// </summary>
[Serializable]
[ProtoContract]
public record OfficeSearchResultDto : IMap
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

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<Office, OfficeSearchResultDto>()
            .ForMember(x => x.OfficeId, x => x.MapFrom(src => src.Id))
            .ForMember(x => x.OfficeName, x => x.MapFrom(src => src.Name));
    }
}
