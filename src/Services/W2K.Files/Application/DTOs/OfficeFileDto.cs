using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.Application.Storage;
using ProtoBuf;

namespace W2K.Files.Application.DTOs;

[Serializable]
[ProtoContract]
public record OfficeFileDto : IMap
{
    /// <summary>
    /// The name of the file.
    /// </summary>
    [ProtoMember(1)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The type of the file.
    /// </summary>
    [ProtoMember(2)]
    public string FileType { get; init; } = string.Empty;

    /// <summary>
    /// The content of the file as a byte array.
    /// </summary>
    [ProtoMember(3)]
    public byte[]? Content { get; init; }

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<UploadedFile, OfficeFileDto>()
            .ForMember(x => x.Name, x => x.MapFrom(src => src.Name))
            .ForMember(x => x.Content, x => x.MapFrom(src => src.Data));
    }
}
