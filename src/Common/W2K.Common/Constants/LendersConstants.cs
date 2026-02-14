namespace DFI.Common.Constants;

public static class LendersConstants
{
    public static readonly string ApplicationName = "DFI.Lenders";

    public static readonly int DefaultCacheExpirationMinutes = 30;

    #region Decision SubStatus Constants
    public static readonly string LoanSubStatusPending = "Pending";

    public static readonly string LoanSubStatusNoOffersAvailable = "No Offers Available";

    public static readonly string LoanSubStatusExpired = "Expired";

    public static readonly string LoanSubStatusOffersAvailable = "Offers Available";

    public static readonly string LoanSubStatusNeedsAttention = "Needs Attention";

    public static readonly string LoanAppSubStatusOfferSelectedPendingDownPayment = "Offer Selected/Pending Down Payment";

    public static readonly string LoanAppSubStatusDownPaymentSelectedPendingPaymentInfo = "Down Payment Selected/Pending Payment Information";

    public static readonly string LoanAppSubStatusPaymentInfoEnteredPendingSignature = "Payment Information Entered/Pending Contract Signature";
    #endregion
}
