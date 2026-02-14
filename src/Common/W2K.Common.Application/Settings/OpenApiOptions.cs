namespace W2K.Common.Application.Settings;

public readonly record struct OpenApiOptions(string Title, string Description, string? Path, bool ShowSessionIdHeader);
