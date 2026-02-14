namespace DFI.Common.Models;

/// <summary>
/// Model containing information about an office owner.
/// </summary>
public readonly record struct OfficeOwnerInfo
{
    /// <summary>
    /// The unique identifier for the office owner.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The owner's first name.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// The owner's middle name, if provided.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// The owner's last name.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// The owner's email address.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// The owner's mobile phone number.
    /// </summary>
    public string? MobilePhone { get; init; }

    /// <summary>
    /// The owner's date of birth.
    /// </summary>
    public string? DOB { get; init; }

    /// <summary>
    /// The owner's social security number.
    /// </summary>
    public string? SSN { get; init; }

    /// <summary>
    /// Ownership percentage of the office.
    /// </summary>
    public short Ownership { get; init; }

    /// <summary>
    /// The owner's annual income.
    /// </summary>
    public int? AnnualIncome { get; init; }

    /// <summary>
    /// The owner's net worth.
    /// </summary>
    public int? NetWorth { get; init; }

    /// <summary>
    /// The owner's home address.
    /// </summary>
    public AddressInfo? HomeAddress { get; init; }
}
