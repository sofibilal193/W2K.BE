#pragma warning disable CA1034 // Nested types should not be visible

namespace DFI.Common.ValueObjects;

public class Address : ValueObject
{
    public string Type { get; private set; } = string.Empty;

    public string Address1 { get; private set; } = string.Empty;

    public string? Address2 { get; private set; }

    public string City { get; private set; } = string.Empty;

    public string? County { get; private set; }

    public string State { get; private set; } = string.Empty;

    public string ZipCode { get; private set; } = string.Empty;

    public string Country { get; private set; } = "USA";

    public string? TimeZone { get; private set; }

    public string? GooglePlaceId { get; private set; }

    public string StreetAddress => string.Concat(Address1, string.IsNullOrEmpty(Address2) ? "" : (" " + Address2));

    public Address(AddressInfo info)
    {
        if (info.Type is not null)
        {
            Type = info.Type;
        }
        if (info.Address1 is not null)
        {
            Address1 = info.Address1;
        }
        Address2 = info.Address2;
        if (info.City is not null)
        {
            City = info.City;
        }
        County = info.County;
        if (info.State is not null)
        {
            State = info.State;
        }
        if (info.ZipCode is not null)
        {
            ZipCode = info.ZipCode;
        }
        if (info.Country is not null)
        {
            Country = info.Country;
        }
        TimeZone = info.TimeZone;
        GooglePlaceId = info.GooglePlaceId;
    }

    protected Address() { }

    public void Update(Address address)
    {
        // Required fields: use null-coalescing to prevent invalid updates
        Address1 = address.Address1 ?? Address1;
        City = address.City ?? City;
        State = address.State ?? State;
        ZipCode = address.ZipCode ?? ZipCode;
        Country = address.Country ?? Country;

        // Optional fields: allow null to support clearing values from FE
        Address2 = address.Address2;
        County = address.County;
        TimeZone = address.TimeZone;
        GooglePlaceId = address.GooglePlaceId;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Type;
        yield return Address1;
        yield return Address2;
        yield return City;
        yield return County;
        yield return State;
        yield return Country;
        yield return ZipCode;
        yield return TimeZone;
        yield return GooglePlaceId;
    }

    public readonly record struct AddressInfo(
        string? Type,
        string? Address1,
        string? Address2,
        string? City,
        string? County,
        string? State,
        string? ZipCode,
        string? Country,
        string? TimeZone,
        string? GooglePlaceId
    );
}

#pragma warning restore CA1034 // Nested types should not be visible
