using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Common.Application.Mappings;
using W2K.Common.Crypto;
using W2K.Identity.Entities;
using ProtoBuf;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
[ProtoContract]

public readonly record struct OfficeInfoDto : IMap
{
    /// <summary>
    /// The name of the office.
    /// </summary>
    [ProtoMember(1)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The legal registered name of the office.
    /// </summary>
    [ProtoMember(2)]
    public string LegalName { get; init; } = string.Empty;

    /// <summary>
    /// The business type (e.g., LLC, Corporation).
    /// </summary>
    [ProtoMember(3)]
    public string BusinessType { get; init; } = string.Empty;

    /// <summary>
    /// Number of years under current ownership.
    /// </summary>
    [ProtoMember(4)]
    public short? YearsCurrentOwnership { get; init; }

    /// <summary>
    /// Federal Tax ID.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string TaxId { get; init; } = string.Empty;

    /// <summary>
    /// License number.
    /// </summary>
    [ProtoMember(6)]
    [JsonEncrypted<string>]
    public string? LicenseNo { get; init; }

    /// <summary>
    /// State where the license is issued.
    /// </summary>
    [ProtoMember(7)]
    public string? LicenseState { get; init; }

    /// <summary>
    /// Office phone number.
    /// </summary>
    [ProtoMember(8)]
    [JsonEncrypted<string>]
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// Office website URL.
    /// </summary>
    [ProtoMember(9)]
    public string? Website { get; init; }

    /// <summary>
    /// Annual revenue of the office.
    /// </summary>
    [ProtoMember(10)]
    public int? AnnualRevenue { get; init; }

    /// <summary>
    /// Office category (e.g., Retail, Service).
    /// </summary>
    [ProtoMember(11)]
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if enrollment is completed.
    /// </summary>
    [ProtoMember(12)]
    public bool IsEnrollmentCompleted { get; init; }

    /// <summary>
    /// Indicates if the office has been reviewed.
    /// </summary>
    [ProtoMember(13)]
    public bool IsReviewed { get; init; }

    /// <summary>
    /// Indicates if the office is currently active.
    /// </summary>
    [ProtoMember(14)]
    public bool IsActive { get; init; }

    /// <summary>
    /// The primary address of the office.
    /// </summary>
    [ProtoMember(15)]
    public AddressDto Address { get; init; } = new();

    public OfficeInfoDto()
    {
    }

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<Office, OfficeInfoDto>()
            .ForMember(x => x.IsActive, x => x.MapFrom(src => !src.IsDisabled))
            .ForMember(x => x.Address, x => x.MapFrom(src => src.Addresses.FirstOrDefault(a => a.Type == IdentityConstants.OfficePrimaryAddressType)));
    }
}
