using W2K.Common.ValueObjects;
using ProtoBuf;

namespace W2K.Identity.Entities;

[Serializable]
[ProtoContract]
public class OfficeGroup : ValueObject
{
    [ProtoMember(1)]
    public int ParentOfficeId { get; private set; }

    [ProtoMember(2)]
    public int OfficeId { get; }

    public OfficeGroup() { }

    public OfficeGroup(int parentOfficeId)
    {
        ParentOfficeId = parentOfficeId;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ParentOfficeId;
        yield return OfficeId;
    }
}
