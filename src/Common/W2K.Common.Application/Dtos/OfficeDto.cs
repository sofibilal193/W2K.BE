using DFI.Common.Crypto;
using DFI.Common.Identity;
using ProtoBuf;

namespace DFI.Common.Application.DTOs;

[Serializable]
[ProtoContract]
public readonly record struct OfficeDto
{
    /// <summary>
    /// Id of the office.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// Type of the office.
    /// </summary>
    [ProtoMember(2)]
    public OfficeType Type { get; init; }

    /// <summary>
    /// Tenant of the Office.
    /// </summary>
    [ProtoMember(3)]
    public string? Tenant { get; init; }

    /// <summary>
    /// Code of the office.
    /// </summary>
    [ProtoMember(5)]
    public string? Code { get; init; }

    /// <summary>
    /// Name of the office.
    /// </summary>
    [ProtoMember(6)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Legal name of the office.
    /// </summary>
    [ProtoMember(7)]
    public string LegalName { get; init; } = string.Empty;

    /// <summary>
    /// Phone number of the office.
    /// </summary>
    [ProtoMember(8)]
    [JsonEncrypted<string>]
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// Fax number of the office.
    /// </summary>
    [ProtoMember(9)]
    [JsonEncrypted<string>]
    public string? Fax { get; init; }

    /// <summary>
    /// Website of the office.
    /// </summary>
    [ProtoMember(10)]
    public string? Website { get; init; }

    /// <summary>
    /// License number of the office.
    /// </summary>
    [ProtoMember(11)]
    [JsonEncrypted<string>]
    public string? LicenseNo { get; init; }

    /// <summary>
    /// License state of the office.
    /// </summary>
    [ProtoMember(12)]
    public string? LicenseState { get; init; }

    /// <summary>
    /// Tax ID of the office.
    /// </summary>
    [ProtoMember(13)]
    [JsonEncrypted<string>]
    public string TaxId { get; init; } = string.Empty;

    /// <summary>
    /// Business type of the office.
    /// </summary>
    [ProtoMember(14)]
    public string BusinessType { get; init; } = string.Empty;

    /// <summary>
    /// Original license date of the office.
    /// </summary>
    [ProtoMember(15)]
    public DateOnly? OriginalLicenseDate { get; init; }

    /// <summary>
    /// Years of current ownership of the office.
    /// </summary>
    [ProtoMember(16)]
    public short? YearsCurrentOwnership { get; init; }

    /// <summary>
    /// Annual revenue of the office.
    /// </summary>
    [ProtoMember(17)]
    public int? AnnualRevenue { get; init; }

    /// <summary>
    /// Promo code used during enrollment.
    /// </summary>
    [ProtoMember(18)]
    public string? PromoCode { get; init; }

    /// <summary>
    /// How the office heard about us.
    /// </summary>
    [ProtoMember(19)]
    public string? HowHeard { get; init; }

    /// <summary>
    /// Is the enrollment process completed?
    /// </summary>
    [ProtoMember(20)]
    public bool IsEnrollmentCompleted { get; init; }

    /// <summary>
    /// Is the office information reviewed?
    /// </summary>
    [ProtoMember(21)]
    public bool IsReviewed { get; init; }

    /// <summary>
    /// Is the office information approved?
    /// </summary>
    [ProtoMember(22)]
    public bool IsApproved { get; init; }

    /// <summary>
    /// UTM source for tracking marketing campaigns.
    /// </summary>
    [ProtoMember(23)]
    public string? UtmSource { get; init; }

    /// <summary>
    /// UTM medium for tracking marketing campaigns.
    /// </summary>
    [ProtoMember(24)]
    public string? UtmMedium { get; init; }

    /// <summary>
    /// UTM campaign for tracking marketing campaigns.
    /// </summary>
    [ProtoMember(25)]
    public string? UtmCampaign { get; init; }

    /// <summary>
    /// Category of the office (e.g., General, Dental, etc.).
    /// </summary>
    [ProtoMember(26)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Addresses of the office.
    /// </summary>
    [ProtoMember(27)]
    public AddressDto Address { get; init; } = new();

    /// <summary>
    /// List of office owners.
    /// </summary>
    [ProtoMember(28)]
    public IList<OfficeOwnerDto> Owners { get; init; } = [];

    /// <summary>
    /// Bank account information.
    /// </summary>
    [ProtoMember(29)]
    public IList<OfficeBankAccountDto> BankAccounts { get; init; } = [];

    public OfficeDto()
    {
    }
}
