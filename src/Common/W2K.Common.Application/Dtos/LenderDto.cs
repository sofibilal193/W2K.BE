using ProtoBuf;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
[ProtoContract]
public readonly record struct LenderDto
{
    /// <summary>
    /// Id of the Lender
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// Name of the Lender.
    /// </summary>
    [ProtoMember(2)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Code of the Lender.
    /// </summary>
    [ProtoMember(3)]
    public string Code { get; init; } = string.Empty;

    public LenderDto()
    {
    }
}
