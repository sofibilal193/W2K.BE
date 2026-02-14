namespace DFI.Common.Models;

/// <summary>
/// Model containing information about an office.
/// </summary>
public readonly record struct OfficeInfo
{
    /// <summary>
    /// The unique identifier for the office.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The display name of the office.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The legal name of the office.
    /// </summary>
    public string LegalName { get; init; }

    /// <summary>
    /// The address of the office.
    /// </summary>
    public AddressInfo? Address { get; init; }

    /// <summary>
    /// The contact phone number for the office.
    /// </summary>
    public string Phone { get; init; }

    /// <summary>
    /// The tax identification number for the office.
    /// </summary>
    public string? TaxId { get; init; }

    /// <summary>
    /// The entity type of the businsess.
    /// </summary>
    public string? BusinessType { get; init; }

    /// <summary>
    /// The state where the business is licensed.
    /// </summary>
    public string? LicenseState { get; init; }

    /// <summary>
    /// The original license date of the business.
    /// </summary>
    public DateOnly? OriginalLicenseDate { get; init; }

    /// <summary>
    /// The number of years the current ownership has been in place.
    /// </summary>
    public short? YearsCurrentOwnership { get; init; }
}
