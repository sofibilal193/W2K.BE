using DFI.Common.Enums;

namespace DFI.Common.Models;

/// <summary>
/// Model containing information about a borrower.
/// </summary>
public readonly record struct BorrowerInfo
{
    public int Id { get; init; }

    /// <summary>
    /// The type of borrower (Primary, ServiceReceiver, CoSigner, CoBorrower, Spouse, Guarantor)
    /// </summary>
    public BorrowerType Type { get; init; }

    /// <summary>
    /// The relationship of this borrower to the primary borrower (if any).
    /// </summary>
    public RelationshipToCustomer? Relationship { get; init; }

    /// <summary>
    /// The first name of the borrower.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The middle name of the borrower.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// The last name of the borrower.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// The email address of the borrower.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// The mobile phone number of the borrower.
    /// </summary>
    public string? MobilePhone { get; init; }

    /// <summary>
    /// The home phone number of the borrower.
    /// </summary>
    public string? HomePhone { get; init; }

    /// <summary>
    /// The Social Security Number of the borrower.
    /// </summary>
    public string? SSN { get; init; }

    /// <summary>
    /// The date of birth of the borrower.
    /// </summary>
    public DateOnly? DOB { get; init; }

    /// <summary>
    /// The residence status of the borrower (Rent, Own, Other).
    /// </summary>
    public ResidenceStatus? ResidenceStatus { get; init; }

    /// <summary>
    /// The monthly rent or mortgage amount for the borrower's residence.
    /// </summary>
    public decimal? MonthlyRentMortgageAmount { get; init; }

    /// <summary>
    /// The address line 1 of the borrower.
    /// </summary>
    public string Address1 { get; init; }

    /// <summary>
    /// The address line 2 of the borrower.
    /// </summary>
    public string? Address2 { get; init; }

    /// <summary>
    /// The city of the borrower.
    /// </summary>
    public string City { get; init; }

    /// <summary>
    /// The state of the borrower.
    /// </summary>
    public string State { get; init; }

    /// <summary>
    /// The zip code of the borrower.
    /// </summary>
    public string ZipCode { get; init; }

    /// <summary>
    /// The total annual income of the borrower.
    /// </summary>
    public decimal? TotalAnnualIncomeAmount { get; init; }

    /// <summary>
    /// The total monthly income of the borrower, calculated from the annual income.
    /// </summary>
    public readonly decimal? TotalMonthlyIncomeAmount => TotalAnnualIncomeAmount.HasValue ? Math.Round(TotalAnnualIncomeAmount.Value / 12, 2) : null;

    /// <summary>
    /// The number of months the borrower has been at their current job.
    /// </summary>
    public int? TimeAtJobMonths { get; init; }

    /// <summary>
    /// The employment status of the borrower (FullTime, PartTime, SelfEmployed, Retired, Student, UnEmployed).
    /// </summary>
    public EmploymentStatus? EmploymentStatus { get; init; }
}
