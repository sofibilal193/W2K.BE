namespace DFI.Common.Models;

/// <summary>
/// Model containing information about a loan decision offer.
/// </summary>
public record OfferInfo
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
    /// The product type code for the offer.
    /// </summary>
    public string? ProductTypeCode { get; init; }

    /// <summary>
    /// The product type name for the offer.
    /// </summary>
    public string? ProductTypeName { get; init; }

    /// <summary>
    /// The minimum loan amount available for the offer.
    /// </summary>
    public decimal? MinAmount { get; init; }

    /// <summary>
    /// The maximum loan amount available for the offer.
    /// </summary>
    public decimal? MaxAmount { get; init; }

    /// <summary>
    /// The loan amount for the offer.
    /// </summary>
    public decimal? Amount { get; init; }

    /// <summary>
    /// The down payment amount for the offer.
    /// </summary>
    public decimal? DownPayment { get; init; }

    /// <summary>
    /// The term (in months) for the offer.
    /// </summary>
    public int? Term { get; init; }

    /// <summary>
    /// The promotional term (in months) for the offer.
    /// </summary>
    public int? PromoTerm { get; init; }

    /// <summary>
    /// The no-interest term (in months) for the offer.
    /// </summary>
    public int? NoInterestTerm { get; init; }

    /// <summary>
    /// The interest rate for the offer.
    /// </summary>
    public decimal? InterestRate { get; init; }

    /// <summary>
    /// The annual percentage rate (APR) for the offer.
    /// </summary>
    public decimal? APR { get; init; }

    /// <summary>
    /// The promotional APR for the offer.
    /// </summary>
    public decimal? PromoAPR { get; init; }

    /// <summary>
    /// The minimum monthly payment for the offer.
    /// </summary>
    public decimal? MinimumMonthlyPayment { get; init; }

    /// <summary>
    /// The promotional monthly payment for the offer.
    /// </summary>
    public decimal? PromoMonthlyPayment { get; init; }

    /// <summary>
    /// The no-interest monthly payment for the offer.
    /// </summary>
    public decimal? NoInterestMonthlyPayment { get; init; }

    /// <summary>
    /// Indicates if this offer is selected.
    /// </summary>
    public bool IsSelected { get; init; }

    /// <summary>
    /// Indicates if this offer is the default offer.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// The origination fee for the offer.
    /// </summary>
    public decimal? OriginationFee { get; init; }

    /// <summary>
    /// The URL for more information about the offer.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// The description of the offer.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The total interest for the offer.
    /// </summary>
    public decimal? TotalInterest { get; init; }

    /// <summary>
    /// The total cost for the offer.
    /// </summary>
    public decimal? TotalCost { get; init; }

    /// <summary>
    /// The discount rate for the offer.
    /// </summary>
    public decimal? DiscountRate { get; private set; }

    /// <summary>
    /// The discount amount for the offer.
    /// </summary>
    public decimal? DiscountAmount { get; private set; }

    /// <summary>
    /// The office discount rate for the offer.
    /// </summary>
    public decimal? OfficeDiscountRate { get; private set; }

    /// <summary>
    /// The office discount amount for the offer.
    /// </summary>
    public decimal? OfficeDiscountAmount { get; private set; }

    public void SetDiscounts(decimal? discountRate, decimal? officeDiscountRate)
    {
        DiscountRate = discountRate;
        DiscountAmount = Amount.HasValue && discountRate.HasValue
            ? Math.Round(Amount.Value * discountRate.Value, 2, MidpointRounding.ToEven)
            : null;
        OfficeDiscountRate = officeDiscountRate;
        OfficeDiscountAmount = Amount.HasValue && officeDiscountRate.HasValue
            ? Math.Round(Amount.Value * officeDiscountRate.Value, 2, MidpointRounding.ToEven)
            : null;
    }
}
