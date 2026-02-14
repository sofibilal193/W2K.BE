namespace DFI.Common.Models;

public readonly record struct OfficeSettlementDetail
{
    public string LenderAppId { get; init; }

    public string? LoanNumber { get; init; }

    public string? Product { get; init; }

    public DateOnly? ServiceDate { get; init; }

    public decimal TotalAmount { get; init; }

    public decimal DownPaymentAmount { get; init; }

    public decimal FinancedAmount { get; init; }

    public decimal ContractAdjustmentAmount { get; init; }

    public int ContractAdjustmentCount { get; init; }

    public decimal CurrentAPR { get; init; }

    public decimal OriginalAPR { get; init; }

    public int Term { get; init; }

    public decimal MonthlyPaymentAmount { get; init; }

    public DateOnly FirstPaymentDate { get; init; }

    public decimal DiscountRate { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal NetFundedAmount { get; init; }

    public decimal OfficeDiscountRate { get; init; }

    public decimal OfficeDiscountAmount { get; init; }

    public decimal NetOfficeFundedAmount { get; init; }

    public bool IsCancelled { get; init; }
}
