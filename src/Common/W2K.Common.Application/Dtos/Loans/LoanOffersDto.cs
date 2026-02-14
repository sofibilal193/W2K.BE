#pragma warning disable IDE0130 // Namespace does not match folder structure

using DFI.Common.Crypto;
using DFI.Common.Enums;

namespace DFI.Common.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// DTO representing a single loan offer displayed on the offer screen.
/// </summary>
public record LoanOffersDto
{
    /// <summary>
    /// The name of the office.
    /// </summary>
    public string? OfficeName { get; init; }

    /// <summary>
    /// The full name of the borrower.
    /// </summary>
    public string? BorrowerFullName { get; init; }

    /// <summary>
    /// The email of the borrower.
    /// </summary>
    [JsonEncrypted<string>]
    public string? BorrowerEmail { get; init; }

    /// <summary>
    /// The phone number of the borrower.
    /// </summary>
    [JsonEncrypted<string>]
    public string? BorrowerPhone { get; init; }

    /// <summary>
    /// The full name of the service receiver.
    /// </summary>
    public string? ServiceReceiverName { get; init; }

    /// <summary>
    /// The relationship of the primary borrower to the service receiver.
    /// </summary>
    public RelationshipToCustomer? Relationship { get; init; }

    /// <summary>
    /// The name of the lender.
    /// </summary>
    public string? LenderName { get; init; }

    /// <summary>
    /// The amount approved by the lender for the loan application.
    /// </summary>
    public decimal? ApprovedAmount { get; init; }

    /// <summary>
    /// The expiration date and time (UTC) for the decision and offers.
    /// </summary>
    public DateTime? ExpirationDateTimeUtc { get; init; }

    /// <summary>
    /// indicates if the loan application Decision is approved.
    /// </summary>
    public bool IsApproved { get; init; }

    /// <summary>
    /// Whether the loan application offers have expired or not.
    /// </summary>
    public bool IsExpired { get; init; }

    /// <summary>
    /// The collection of available loan offers for the approved application.
    /// </summary>
    public IReadOnlyCollection<LoanAppOffersDto>? Offers { get; init; }
}

public record LoanAppOffersDto
{
    /// <summary>
    /// The product code for the offer.
    /// </summary>
    public string? ProductCode { get; init; }

    /// <summary>
    /// The product name for the offer.
    /// </summary>
    public string? ProductName { get; init; }

    /// <summary>
    /// The loan amount for this specific offer.
    /// </summary>
    public decimal? LoanAmount { get; init; }

    /// <summary>
    /// The amount due today (down payment).
    /// </summary>
    public decimal? AmountDueToday { get; init; }

    /// <summary>
    /// The loan term in months.
    /// </summary>
    public int? TermMonths { get; init; }

    /// <summary>
    /// The Annual Percentage Rate as a percentage.
    /// </summary>
    public decimal? APRPercentage { get; init; }

    /// <summary>
    /// The estimated minimum monthly payment.
    /// </summary>
    public decimal EstimatedMinPaymentAmount { get; init; }

    /// <summary>
    /// The URL to view or accept the offer.
    /// </summary>
    public string? OfferUrl { get; init; }
}
