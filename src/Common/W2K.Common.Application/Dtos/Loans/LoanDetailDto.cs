using DFI.Common.Crypto;
using ProtoBuf;

namespace DFI.Common.Application.DTOs.Loans;

/// <summary>
/// Loan Details
/// </summary>
[Serializable]
[ProtoContract]
public record LoanDetailDto
{
    /// <summary>
    /// The full name of the borrower.
    /// </summary>
    [ProtoMember(1)]
    public string BorrowerName { get; init; } = string.Empty;

    /// <summary>
    /// The status of the loan.
    /// Possible values: NotApplicable, InProgress, NeedsAttention, Signed, SubmittedForFunding, Funded, FundsReleased, Booked, Expired, Cancelled, Refund.
    /// </summary>
    [ProtoMember(2)]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// The ID of the loan application.
    /// </summary>
    [ProtoMember(3)]
    public int? LoanAppId { get; init; }

    /// <summary>
    /// The original amount of the loan (Also as OriginalLoanAmount in Refund).
    /// </summary>
    [ProtoMember(4)]
    public decimal LoanAmount { get; init; }

    /// <summary>
    /// The discount amount applied to the loan (also MerchantFee).
    /// </summary>
    [ProtoMember(5)]
    public decimal? DiscountAmount { get; init; }

    /// <summary>
    /// The discount percentage applied to the loan (also MerchantFee).
    /// </summary>
    [ProtoMember(6)]
    public decimal? DiscountPercentage { get; init; }

    /// <summary>
    /// The net funded amount for the loan after discounts (Also as DisbursementAmount in Refund).
    /// </summary>
    [ProtoMember(7)]
    public decimal NetAmount { get; init; }

    /// <summary>
    /// The ID of the lender.
    /// </summary>
    [ProtoMember(8)]
    public int LenderId { get; init; }

    /// <summary>
    /// The ID of the loan.
    /// </summary>
    [ProtoMember(22)]
    public int LoanId { get; init; }

    /// The ID of the office.
    /// </summary>
    [ProtoMember(20)]
    public int OfficeId { get; init; }

    /// <summary>
    /// The name of the lender.
    /// </summary>
    [ProtoMember(9)]
    public string LenderName { get; init; } = string.Empty;

    /// <summary>
    /// The identifier for the application in the lender's external system.
    /// </summary>
    [ProtoMember(21)]
    public string? LenderAppId { get; init; }

    /// <summary>
    /// The office discount percentage applied to the loan.
    /// </summary>
    [ProtoMember(23)]
    public decimal OfficeDiscountPercentage { get; init; }

    /// <summary>
    /// Identifier for the loan in the lender's system.
    /// </summary>
    [ProtoMember(10)]
    public string? LenderLoanId { get; init; }

    /// <summary>
    /// The name of the office.
    /// </summary>
    [ProtoMember(11)]
    public string OfficeName { get; init; } = string.Empty;

    /// <summary>
    /// The email of the primary borrower.
    /// </summary>
    [ProtoMember(12)]
    [JsonEncrypted<string>]
    public string? BorrowerEmail { get; init; }

    /// <summary>
    /// The phone number of the primary borrower.
    /// </summary>
    [ProtoMember(13)]
    [JsonEncrypted<string>]
    public string? BorrowerPhone { get; init; }

    /// <summary>
    /// The date (UTC) when the loan application was created.
    /// </summary>
    [ProtoMember(14)]
    public DateTime? ApplicationDateTimeUtc { get; init; }

    /// <summary>
    /// The date (UTC) when the loan was created.
    /// </summary>
    [ProtoMember(15)]
    public DateTime? LoanDateTimeUtc { get; init; }

    /// <summary>
    /// The date (UTC) when the loan was last updated.
    /// </summary>
    [ProtoMember(16)]
    public DateTime? LastUpdatedDateTimeUtc { get; init; }

    /// <summary>
    /// The full name of the person receiving service (if different from the borrower).
    /// </summary>
    [ProtoMember(17)]
    public string? ServiceFullName { get; init; }

    /// <summary>
    /// The total refund amount for the loan (when in Refund stage).
    /// </summary>
    [ProtoMember(18)]
    public decimal? RefundAmount { get; init; }

    /// <summary>
    /// The remaining loan balance after refund (when in Refund stage).
    /// </summary>
    [ProtoMember(19)]
    public decimal? LoanBalance { get; init; }

    /// <summary>
    /// The date the loan was funded to the office.
    /// </summary>
    [ProtoMember(20)]
    public DateTime? FundedDate { get; init; }

    /// <summary>
    /// The stage of the loan.
    /// </summary>
    [ProtoMember(21)]
    public string Stage { get; init; } = string.Empty;
}
