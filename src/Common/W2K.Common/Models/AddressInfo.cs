namespace DFI.Common.Models;

/// <summary>
/// Model containing information about an address.
/// </summary>
public readonly record struct AddressInfo
{
    /// <summary>
    /// The first line of the address.
    /// </summary>
    public string Address1 { get; init; }

    /// <summary>
    /// The second line of the address.
    /// </summary>
    public string? Address2 { get; init; }

    /// <summary>
    /// The city of the address.
    /// </summary>
    public string City { get; init; }

    /// <summary>
    /// The state of the address.
    /// </summary>
    public string State { get; init; }

    /// <summary>
    /// The zip code of the address.
    /// </summary>
    public string ZipCode { get; init; }
}
