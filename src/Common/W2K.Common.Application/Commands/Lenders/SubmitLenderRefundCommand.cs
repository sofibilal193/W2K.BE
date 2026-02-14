using System.Text.Json.Serialization;
using DFI.Common.Models.Lenders;
using MediatR;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace DFI.Common.Application.Lenders;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Command to submit a refund request to a lender for a previously funded loan.
/// </summary>
public record SubmitLenderRefundCommand : IRequest<SubmitRefundResponse>
{
    /// <summary>
    /// The ID of the Lender.
    /// </summary>
    public required int LenderId { get; init; }

    /// <summary>
    /// The ID of the Office.
    /// </summary>
    [JsonIgnore]
    public int OfficeId { get; private set; }

    /// <summary>
    /// The ID of the Loan.
    /// </summary>
    public required int LoanId { get; init; }

    /// <summary>
    /// The identifier for the loan in the lender's system.
    /// </summary>
    public required string LenderAppId { get; init; }

    /// <summary>
    /// The lender's loan number/ID for the funded loan.
    /// </summary>
    public string? LenderLoanId { get; init; }

    /// <summary>
    /// The ID of the original loan application (optional).
    /// </summary>
    public int? LoanAppId { get; init; }

    /// <summary>
    /// Flag indicating whether this is a partial refund (true) or full refund/cancellation (false).
    /// </summary>
    public required bool IsPartialRefund { get; init; }

    /// <summary>
    /// The amount to refund.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// The reason for the refund.
    /// </summary>
    public required string Reason { get; init; }

    public void SetIds(int officeId)
    {
        OfficeId = officeId;
    }
}
