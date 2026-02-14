namespace W2K.Config.Entities;

public record FieldValue
{
    public string Value { get; init; } = string.Empty;

    public string? Text { get; init; }

    public string? Description { get; init; }

    public short DisplayOrder { get; init; }
}

