using System.Text.Json.Serialization;
using W2K.Common.Application.Commands;
using W2K.Common.Crypto;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpsertOfficeCommand : IRequest<int?>
{
    [JsonIgnore]
    public int? OfficeId { get; private set; }

    public string? Tenant { get; init; }

    public string Category { get; init; } = string.Empty;

    public string? Code { get; init; }

    public string Name { get; init; } = string.Empty;

    public string LegalName { get; init; } = string.Empty;

    [JsonEncrypted<string>]
    public string Phone { get; init; } = string.Empty;

    [JsonEncrypted<string>]
    public string? Fax { get; init; }

    public string? Website { get; init; }

    [JsonEncrypted<string>]
    public string? LicenseNo { get; init; }

    public string? LicenseState { get; init; }

    public DateOnly? OriginalLicenseDate { get; init; }

    public short? YearsCurrentOwnership { get; init; }

    [JsonEncrypted<string>]
    public string TaxId { get; init; } = string.Empty;

    public string BusinessType { get; init; } = string.Empty;

    public int? AnnualRevenue { get; init; }

    public string? PromoCode { get; init; }

    public string? HowHeard { get; init; }

    public string? UtmSource { get; init; }

    public string? UtmMedium { get; init; }

    public string? UtmCampaign { get; init; }

    public required bool IsFirstTimeEnroll { get; init; }

    public bool? IsEnrollmentCompleted { get; init; }

    public bool? IsReviewed { get; init; }

    public bool? IsActive { get; init; }

    public AddressCommand Address { get; init; } = new();

    public AdminUserCommand AdminUser { get; init; } = new();

    public void SetOfficeId(int officeId)
    {
        OfficeId = officeId;
    }
}
