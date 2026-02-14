using ProtoBuf;
using W2K.Common.Application.Mappings;
using W2K.Common.ValueObjects;

namespace W2K.Files.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct TagDto : IMapFrom<Tag>
{
    /// <summary>
    /// The name of the tag.
    /// </summary>
    [ProtoMember(1)]
    public string Name { get; init; }

    /// <summary>
    /// The value of the tag.
    /// </summary>
    [ProtoMember(2)]
    public string Value { get; init; }
}
