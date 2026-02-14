using System.Text.Json.Serialization;
using W2K.Common.Crypto;
using W2K.Common.Enums;
using ProtoBuf;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
///Application Details
/// </summary>
[Serializable]
[ProtoContract]
public record LoanAppDetailDto
{
    /// <summary>
    /// The ID of the loan application.
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; init; }

    /// <summary>
    /// The current status of the loan application decision.
    /// </summary>
    [ProtoMember(2)]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// The sub status of the loan application decision.
    /// </summary>
    [ProtoMember(3)]
    public string? SubStatus { get; init; } = string.Empty;

    /// <summary>
    /// The original amount requested by the borrower.
    /// </summary>
    [ProtoMember(4)]
    public decimal? RequestedAmount { get; init; }

    /// <summary>
    /// The amount approved by the lender.
    /// </summary>
    [ProtoMember(5)]
    public decimal? ApprovedAmount { get; init; }

    /// <summary>
    /// The date (UTC) when the application was submitted.
    /// </summary>
    [ProtoMember(6)]
    public DateTime? ApplicationDateTimeUtc { get; init; }

    /// <summary>
    /// The date and time (UTC) when the application was last updated.
    /// </summary>
    [ProtoMember(7)]
    public DateTime? LastUpdatedDateTimeUtc { get; init; }

    /// <summary>
    /// The expiration date and time (UTC) for the decision and offers.
    /// </summary>
    [ProtoMember(8)]
    public DateTime? ExpirationDateTimeUtc { get; init; }

    // Customer information
    /// <summary>
    /// The full name of the primary borrower.
    /// </summary>
    [ProtoMember(9)]
    public string? BorrowerName { get; init; }

    /// <summary>
    /// The phone number of the primary borrower on the application.
    /// </summary>
    [ProtoMember(10)]
    [JsonEncrypted<string>]
    public string? BorrowerPhone { get; init; }

    /// <summary>
    /// The email address of the primary borrower on the application.
    /// </summary>
    [ProtoMember(11)]
    [JsonEncrypted<string>]
    public string? BorrowerEmail { get; init; }

    /// <summary>
    /// The full name of the service receiver.
    /// </summary>
    [ProtoMember(12)]
    public string? ServiceReceiverName { get; init; }

    /// <summary>
    /// The relationship of the primary borrower to the service receiver.
    /// </summary>
    [ProtoMember(13)]
    public RelationshipToCustomer? Relationship { get; init; }

    // Office information
    /// <summary>
    /// The ID of the office.
    /// </summary>
    [ProtoMember(20)]
    public int OfficeId { get; init; }

    /// <summary>
    /// The name of the office.
    /// </summary>
    [ProtoMember(14)]
    public string OfficeName { get; init; } = string.Empty;

    // Lender information
    /// <summary>
    /// The Lender App Id of the lender.
    /// </summary>
    [ProtoMember(15)]
    public string? LenderAppId { get; init; }

    /// <summary>
    /// The name of the lender.
    /// </summary>
    [ProtoMember(16)]
    public string? LenderName { get; init; }

    /// <summary>
    /// The reason for Decision.
    /// </summary>
    [ProtoMember(17)]
    public string? Reason { get; init; }

    // Offers
    /// <summary>
    /// List of offers associated with the loan application.
    /// </summary>
    [ProtoMember(18)]
    public IReadOnlyCollection<LoanAppOffersDto>? Offers { get; init; }

    /// <summary>
    /// The number of offers available.
    /// </summary>
    [ProtoMember(19)]
    public int OfferCount => Offers?.Count ?? 0;

    /// <summary>
    /// The stage of the loan application.
    /// </summary>
    [ProtoMember(21)]
    public string Stage { get; init; } = string.Empty;

}
