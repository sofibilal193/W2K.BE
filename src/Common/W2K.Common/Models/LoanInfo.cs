using W2K.Common.Enums;

namespace W2K.Common.Models;

/// <summary>
/// Model containing information about a loan.
/// </summary>
public readonly record struct LoanInfo
{
    /// <summary>
    /// The unique identifier for the loan.
    /// </summary>
    public int? Id { get; init; }

    /// <summary>
    /// The unique identifier for the office.
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// The name of the office.
    /// </summary>
    public string? OfficeName { get; init; }

    /// <summary>
    /// The Id of the original application.
    /// </summary>
    public int? LoanAppId { get; init; }

    /// <summary>
    /// The date and time when the loan application was created (UTC).
    /// </summary>
    public DateTime? LoanAppDateTimeUtc { get; init; }

    /// <summary>
    /// The Id of the lender.
    /// </summary>
    public int? LenderId { get; init; }

    /// <summary>
    /// The name of the lender.
    /// </summary>
    public string? LenderName { get; init; }

    /// <summary>
    /// Identifier for the application in the lender's system.
    /// </summary>
    public string? LenderAppId { get; init; }

    /// <summary>
    /// Url for the application in the lender's system.
    /// </summary>
    public string? LenderAppUrl { get; init; }

    /// <summary>
    /// Identifier for the loan in the lender's system.
    /// </summary>
    public string? LenderLoanId { get; init; }

    /// <summary>
    /// Url for the loan in the lender's system.
    /// </summary>
    public string? LenderLoanUrl { get; init; }

    /// <summary>
    /// An optional reference code assigned by the office for this loan.
    /// </summary>
    public string? OfficeReferenceCode { get; init; }

    /// <summary>
    /// The first name of the primary borrower.
    /// </summary>
    public string? PrimaryFirstName { get; init; }

    /// <summary>
    /// The last name of the primary borrower.
    /// </summary>
    public string? PrimaryLastName { get; init; }

    /// <summary>
    /// The email address of the borrower.
    /// </summary>
    public string PrimaryEmail { get; init; } = string.Empty;

    /// <summary>
    /// The phone number of the borrower.
    /// </summary>
    public string PrimaryPhone { get; init; } = string.Empty;

    /// <summary>
    /// The first name of the secondary borrower.
    /// </summary>
    public string? SecondaryFirstName { get; init; }

    /// <summary>
    /// The last name of the secondary borrower.
    /// </summary>
    public string? SecondaryLastName { get; init; }

    /// <summary>
    /// The relationship of the primary borrower to the service receiver.
    /// </summary>
    public RelationshipToCustomer? Relationship { get; init; }

    /// <summary>
    /// The first name of the person who is receiving the service (e.g., loan).
    /// If this person is not the borrower, it is typically the patient or service recipient.
    /// </summary>
    public string? ServiceFirstName { get; init; }

    /// <summary>
    /// The last name of the person who is receiving the service (e.g., loan).
    /// If this person is not the borrower, it is typically the patient or service recipient.
    /// </summary>
    public string? ServiceLastName { get; init; }

    /// <summary>
    /// The name of the service.
    /// </summary>
    public string? ServiceName { get; init; }

    /// <summary>
    /// The date of the service.
    /// </summary>
    public DateOnly? ServiceDate { get; init; }

    /// <summary>
    /// The product code for the loan.
    /// </summary>
    public string? ProductCode { get; init; }

    /// <summary>
    /// The product name for the loan.
    /// </summary>
    public string? ProductName { get; init; }

    /// <summary>
    /// The product type code for the loan.
    /// </summary>
    public string? ProductTypeCode { get; init; }

    /// <summary>
    /// The product type name for the loan.
    /// </summary>
    public string? ProductTypeName { get; init; }

    /// <summary>
    /// The original amount of the loan.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// The down payment amount for the loan, if any.
    /// </summary>
    public decimal? DownPayment { get; init; }

    /// <summary>
    /// The term (in months) for the loan.
    /// </summary>
    public int? Term { get; init; }

    /// <summary>
    /// The promotional term (in months) for the loan.
    /// </summary>
    public int? PromoTerm { get; init; }

    /// <summary>
    /// The no-interest term (in months) for the loan.
    /// </summary>
    public int? NoInterestTerm { get; init; }

    /// <summary>
    /// The interest rate for the loan.
    /// </summary>
    public decimal? InterestRate { get; init; }

    /// <summary>
    /// The annual percentage rate (APR) for the loan.
    /// </summary>
    public decimal? APR { get; init; }

    /// <summary>
    /// The promotional APR for the loan.
    /// </summary>
    public decimal? PromoAPR { get; init; }

    /// <summary>
    /// The minimum monthly payment for the loan.
    /// </summary>
    public decimal? MinimumMonthlyPayment { get; init; }

    /// <summary>
    /// The promotional monthly payment for the loan.
    /// </summary>
    public decimal? PromoMonthlyPayment { get; init; }

    /// <summary>
    /// The no-interest monthly payment for the loan.
    /// </summary>
    public decimal? NoInterestMonthlyPayment { get; init; }

    /// <summary>
    /// The origination fee for the loan.
    /// </summary>
    public decimal? OriginationFee { get; init; }

    /// <summary>
    /// The description for the loan, if any.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The total interest for the loan.
    /// </summary>
    public decimal? TotalInterest { get; init; }

    /// <summary>
    /// The total cost for the loan.
    /// </summary>
    public decimal? TotalCost { get; init; }

    /// <summary>
    /// The discount rate for the loan.
    /// </summary>
    public decimal? DiscountRate { get; init; }

    /// <summary>
    /// The discount amount for the loan.
    /// </summary>
    public decimal? DiscountAmount { get; init; }

    /// <summary>
    /// The office discount rate for the loan.
    /// </summary>
    public decimal? OfficeDiscountRate { get; init; }

    /// <summary>
    /// The office discount amount for the loan.
    /// </summary>
    public decimal? OfficeDiscountAmount { get; init; }

    /// <summary>
    /// The current status of the loan.
    /// </summary>
    public LoanStatus Status { get; init; }

    /// <summary>
    /// Indicates if the Service Receiver is the same as the Borrower.
    /// </summary>
    public bool IsServiceReceiverSameAsBorrower => PrimaryFirstName == ServiceFirstName && PrimaryLastName == ServiceLastName;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoanInfo"/> struct with default values.
    /// </summary>
    public LoanInfo()
    {
    }
}
