using DFI.Common.Utils;

namespace DFI.Common.Infrastructure.Settings;

public record HttpClientLoggingSettings
{
    public bool IsMaskingEnabled { get; init; }

    public int AuditLogTTLDays { get; init; } = 30;

    public string Mask { get; init; } = "*****";

    public Dictionary<string, MaskedFieldSet> MaskedFields { get; init; } = [];
}

public record MaskedFieldSet
{
    public string ProviderCode { get; init; } = "";

    public LogContentType ContentType { get; init; }

    public IEnumerable<string> FieldPaths { get; init; } = [];
}
