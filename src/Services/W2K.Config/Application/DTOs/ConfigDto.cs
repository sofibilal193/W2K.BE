using ConfigEntity = W2K.Config.Entities.Config;
using W2K.Common.Application.Mappings;
using ProtoBuf;
namespace W2K.Config.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct ConfigDto : IMapFrom<ConfigEntity>
{
    [ProtoMember(1)]
    public string Type { get; init; }

    [ProtoMember(2)]
    public string Name { get; init; }

    [ProtoMember(3)]
    public string? Description { get; init; }

    [ProtoMember(4)]
    public string? Value { get; init; }

    [ProtoMember(5)]
    public short DisplayOrder { get; init; }
}
