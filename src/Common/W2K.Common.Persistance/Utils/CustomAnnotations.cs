using System.Diagnostics.CodeAnalysis;

namespace DFI.Common.Persistence.Utils;

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public static class CustomAnnotations
{
    public static readonly string HistoryRetention = "HistoryRetention";

    public static readonly string Encryption = "Encryption";

    public static readonly string SensitivityClassification = "SensitivityClassification";

    public static readonly string DynamicDataMask = "DynamicDataMask";
}
