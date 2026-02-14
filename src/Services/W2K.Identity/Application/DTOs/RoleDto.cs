using W2K.Common.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record RoleDto : IMapFrom<Role>
{
    /// <summary>
    /// Id of the role.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// Name of the role.
    /// </summary>
    [ProtoMember(2)]
    public string Name { get; init; } = string.Empty;
}

