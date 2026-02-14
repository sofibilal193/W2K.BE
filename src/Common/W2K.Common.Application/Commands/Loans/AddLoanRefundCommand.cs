using System.Text.Json.Serialization;
using W2K.Common.Enums;
using MediatR;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Command to update a loan with a new refund record.
/// </summary>
/// <param name="LoanId">The ID of the loan to update.</param>
/// <param name="RefundAmount">The refund amount (gross).</param>
/// <param name="NetRefundAmount">The net refund amount after discounts.</param>
/// <param name="NetOfficeRefundAmount">The net office refund amount after office-specific discounts.</param>
/// <param name="Reason">The reason for the refund.</param>
/// <param name="TicketId">Optional ticket ID associated with the refund.</param>
/// <param name="Status">The status of the refund.</param>
public record struct AddLoanRefundCommand : IRequest
{
    [JsonIgnore]
    public int LoanId { get; private set; }

    public decimal RefundAmount { get; init; }

    public decimal NetRefundAmount { get; init; }

    public decimal NetOfficeRefundAmount { get; init; }

    public string Reason { get; init; }

    public string? TicketId { get; init; }

    public RefundStatus Status { get; init; }

    public void SetIds(int loanId)
    {
        LoanId = loanId;
    }
}
