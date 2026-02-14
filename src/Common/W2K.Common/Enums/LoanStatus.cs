#pragma warning disable CA1720 // Identifiers should not contain type names

namespace DFI.Common.Enums;

public enum LoanStatus
{
    NotApplicable,
    InProgress,
    NeedsAttention,
    Signed,
    SubmittedForFunding,
    Funded,
    FundsReleased,
    Booked,
    Expired,
    Cancelled,
    Refund,
}

#pragma warning restore CA1720 // Identifiers should not contain type names
