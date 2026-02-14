
namespace W2K.Common.ValueObjects;

public class Tag : ValueObject
{
    public string Name { get; private set; } = string.Empty;

    public string? Value { get; private set; }

    public Tag(string name, string? value)
        : this()
    {
        Name = name ?? string.Empty;
        Value = value;
    }

    protected Tag() { }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
