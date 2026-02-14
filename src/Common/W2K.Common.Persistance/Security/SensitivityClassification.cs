using System.Diagnostics.CodeAnalysis;

namespace DFI.Common.Persistence.Security;

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public record SensitivityClassification(string Label, string InformationType, SensitivityRank Rank);

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public static class SensitivityLabels
{
    public static readonly string Public = "Public";
    public static readonly string General = "General";
    public static readonly string Confidential = "Confidential";
    public static readonly string ConfidentialGdpr = "Confidential - GDPR";
    public static readonly string HighlyConfidential = "Highly Confidential";
    public static readonly string HighlyConfidentialGdpr = "Highly Confidential - GDPR";
}

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public static class SensitivityInformationTypes
{
    public static readonly string Banking = "Banking";
    public static readonly string ContactInfo = "Contact Info";
    public static readonly string Credentials = "Credentials";
    public static readonly string CreditCard = "Credit Card";
    public static readonly string DateOfBirth = "Date of Birth";
    public static readonly string Financial = "Financial";
    public static readonly string Health = "Health";
    public static readonly string Name = "Name";
    public static readonly string NationalId = "National ID";
    public static readonly string Networking = "Networking";
    public static readonly string Ssn = "SSN";
    public static readonly string Other = "Other";
}

public enum SensitivityRank
{
    NONE = 0,
    LOW = 10,
    MEDIUM = 20,
    HIGH = 30,
    CRITICAL = 40
}
