using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using DFI.Common.Enums;

namespace DFI.Common.Models;

/// <summary>
/// Model containing information about loan application decision.
/// </summary>
public record DecisionInfo
{
    /// <summary>
    /// The reference ID for the decision in the lender's system.
    /// </summary>
    public string? LenderRefId { get; init; }

    /// <summary>
    /// The status of the decision (Pending, Withdrawn, Duplicate, Declined, Conditioned, Approved, NeedsAttention, Expired).
    /// </summary>
    public DecisionStatus? Status { get; init; }

    /// <summary>
    /// The sub-status providing additional detail about the decision status.
    /// </summary>
    public string? SubStatus { get; init; }

    /// <summary>
    /// The amount requested on the loan application.
    /// </summary>
    public decimal? RequestedAmount { get; private set; }

    /// <summary>
    /// The amount approved by the lender.
    /// </summary>
    public decimal? ApprovedAmount { get; private set; }

    /// <summary>
    /// The expiration date (UTC) for the decision.
    /// </summary>
    public DateOnly? ExpirationDateUtc { get; init; }

    /// <summary>
    /// The offers associated with the decision.
    /// </summary>
    public Collection<OfferInfo>? Offers { get; init; }

    public DecisionInfo()
    {
    }

    [JsonConstructor]
    public DecisionInfo(string? lenderRefId, DecisionStatus? status, string? subStatus, decimal? requestedAmount, decimal? approvedAmount, DateOnly? expirationDateUtc, Collection<OfferInfo>? offers)
    {
        LenderRefId = lenderRefId;
        Status = status;
        SubStatus = subStatus;
        RequestedAmount = requestedAmount;
        ApprovedAmount = approvedAmount;
        ExpirationDateUtc = expirationDateUtc;
        Offers = offers;
    }

    public void SetAmounts(decimal? requestedAmount, decimal? approvedAmount)
    {
        if (requestedAmount.HasValue)
            RequestedAmount = requestedAmount.Value;
        if (approvedAmount.HasValue)
            ApprovedAmount = approvedAmount.Value;
    }
}
