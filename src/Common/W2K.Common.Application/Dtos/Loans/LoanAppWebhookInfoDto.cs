using DFI.Common.Enums;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace DFI.Common.Application.Loans;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record LoanAppWebhookInfoDto
{
    /// <summary>
    /// The Id of the original loan application.
    /// </summary>
    public int LoanAppId { get; init; }

    /// <summary>
    /// The date and time when the loan application was created (UTC).
    /// </summary>
    public DateTime LoanAppDateTimeUtc { get; init; }

    /// <summary>
    /// The Id of the Office.
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// The name of the Office.
    /// </summary>
    public string? OfficeName { get; init; }

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
    /// The first name of the primary borrower.
    /// </summary>
    public string PrimaryFirstName { get; init; } = string.Empty;

    /// <summary>
    /// The last name of the primary borrower.
    /// </summary>
    public string PrimaryLastName { get; init; } = string.Empty;

    /// <summary>
    /// The email address of the primary borrower.
    /// </summary>
    public string PrimaryEmail { get; init; } = string.Empty;

    /// <summary>
    /// The phone number of the primary borrower.
    /// </summary>
    public string PrimaryPhone { get; init; } = string.Empty;

    /// <summary>
    /// The first name of the secondary borrower (co-borrower).
    /// </summary>
    public string? SecondaryFirstName { get; init; }

    /// <summary>
    /// The last name of the secondary borrower (co-borrower).
    /// </summary>
    public string? SecondaryLastName { get; init; }

    /// <summary>
    /// The relationship of the primary borrower to the service receiver.
    /// </summary>
    public RelationshipToCustomer? Relationship { get; init; }

    /// <summary>
    /// The first name of the person who is receiving the service.
    /// </summary>
    public string? ServiceFirstName { get; init; }

    /// <summary>
    /// The last name of the person who is receiving the service.
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
    /// The interest rate for the loan.
    /// </summary>
    public decimal? InterestRate { get; init; }

    /// <summary>
    /// The office discount rate for the loan.
    /// </summary>
    public decimal? OfficeDiscountRate { get; init; }

    /// <summary>
    /// The office discount amount for the loan.
    /// </summary>
    public decimal? OfficeDiscountAmount { get; init; }
}
