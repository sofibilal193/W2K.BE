using AutoMapper;
using W2K.Common.Application.Mappings;
using W2K.Common.Crypto;
using W2K.Common.ValueObjects;
using W2K.Identity.Application.Mappings;
using W2K.Identity.Entities;
using ProtoBuf;

namespace W2K.Identity.Application.DTOs;

[Serializable]
[ProtoContract]
public record OfficeListItemDto : IMap
{
    /// <summary>
    /// The Id of the office.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// The name of the office.
    /// </summary>
    [ProtoMember(2)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The last updated date/time (UTC) of the office record.
    /// </summary>
    [ProtoMember(3)]
    public DateTime? LastUpdatedDateTimeUtc { get; init; }

    /// <summary>
    /// The last login date/time (UTC) for any user in the office.
    /// </summary>
    [ProtoMember(4)]
    public DateTime? LastUserLoginDateTimeUtc { get; init; }

    /// <summary>
    /// The phone number of the office.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string Phone { get; init; } = string.Empty;

    /// <summary>
    /// Indicates whether the office is active.
    /// </summary>
    [ProtoMember(6)]
    public bool IsActive { get; init; }

    /// <summary>
    /// Indicates whether the office enrollment is completed.
    /// </summary>
    [ProtoMember(7)]
    public bool IsEnrollmentCompleted { get; init; }

    /// <summary>
    /// Indicates whether the office is approved.
    /// </summary>
    [ProtoMember(8)]
    public bool IsApproved { get; init; }

    /// <summary>
    /// Street address where the office is located.
    /// </summary>
    [ProtoMember(9)]
    [JsonEncrypted<string>]
    public string Address1 { get; init; } = string.Empty;

    /// <summary>
    /// Suite no or 2nd address line where office is located
    /// </summary>
    [ProtoMember(10)]
    [JsonEncrypted<string>]
    public string Address2 { get; init; } = string.Empty;

    /// <summary>
    /// City where the office is located.
    /// </summary>
    [ProtoMember(11)]
    [JsonEncrypted<string>]
    public string City { get; init; } = string.Empty;

    /// <summary>
    /// State for the office location.
    /// </summary>
    [ProtoMember(12)]
    [JsonEncrypted<string>]
    public string State { get; init; } = string.Empty;

    /// <summary>
    /// Zip code for the office location.
    /// </summary>
    [ProtoMember(13)]
    [JsonEncrypted<string>]
    public string ZipCode { get; init; } = string.Empty;

    /// <summary>
    /// Office enrollment status.
    /// Possible values: Incomplete, Enrolled, Reviewed, Approved, Denied, Inactive.
    /// </summary>
    [ProtoMember(14)]
    public string Status { get; init; } = string.Empty;

    public void Mapping(Profile profile)
    {
        _ = profile.CreateMap<Office, OfficeListItemDto>()
            .ForMember(x => x.LastUpdatedDateTimeUtc, x => x.MapFrom(x => x.ModifyDateTimeUtc))
            .ForMember(x => x.IsActive, x => x.MapFrom(x => !x.IsDisabled))
            .ForMember(x => x.LastUserLoginDateTimeUtc, x => x.MapFrom(x => x.LastLoginDateTimeUtc))
            .ForMember(x => x.Address1, x => x.MapFrom(src => MapAddress1(src)))
            .ForMember(x => x.Address2, x => x.MapFrom(src => MapAddress2(src)))
            .ForMember(x => x.City, x => x.MapFrom(src => MapCity(src)))
            .ForMember(x => x.State, x => x.MapFrom(src => MapState(src)))
            .ForMember(x => x.ZipCode, x => x.MapFrom(src => MapZip(src)))
            .ForMember(x => x.Status, x => x.MapFrom(src => OfficeMappings.MapEnrollmentStatus(src)));
    }

    private static string MapAddress1(Office src)
    {
        return SelectAddress(src)?.Address1 ?? string.Empty;
    }

    private static string MapAddress2(Office src)
    {
        return SelectAddress(src)?.Address2 ?? string.Empty;
    }

    private static string MapCity(Office src)
    {
        return SelectAddress(src)?.City ?? string.Empty;
    }

    private static string MapState(Office src)
    {
        return SelectAddress(src)?.State ?? string.Empty;
    }

    private static string MapZip(Office src)
    {
        return SelectAddress(src)?.ZipCode ?? string.Empty;
    }

    private static Address? SelectAddress(Office src)
    {
        return src.Addresses?.FirstOrDefault(x => x.Type == IdentityConstants.OfficePrimaryAddressType);
    }
}
