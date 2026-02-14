using W2K.Common.Application.Mappings;
using ProtoBuf;

namespace W2K.Files.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct OfficeFilesDto : IMapFrom<Entities.File>
{
    /// <summary>
    /// The unique identifier of the file.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// The label of the file.
    /// </summary>
    [ProtoMember(2)]
    public string Label { get; init; }

    /// <summary>
    /// The content type of the file.
    /// </summary>
    [ProtoMember(3)]
    public string ContentType { get; init; }

    /// <summary>
    /// The last modified date and time of the file in UTC.
    /// </summary>
    [ProtoMember(4)]
    public DateTime ModifyDateTimeUtc { get; init; }

    /// <summary>
    /// The collection of tags associated with the file.
    /// </summary>
    [ProtoMember(5)]
    public IReadOnlyCollection<TagDto> Tags { get; init; }
}
