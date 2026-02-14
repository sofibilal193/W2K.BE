using W2K.Common.ValueObjects;

namespace W2K.Identity.Entities;

public class OfficeOwner : BaseEntityValueObject
{
    public virtual Address? Address { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string? MobilePhone { get; private set; }

    public string? DOB { get; private set; }

    public string? SSN { get; private set; }

    public short Ownership { get; private set; }

    public int? AnnualIncome { get; private set; }

    public int? NetWorth { get; private set; }

    public DateOnly? DOBDate => DateOnly.TryParse(DOB, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var date)
                                    ? date
                                    : null;

    // Constructors
    public OfficeOwner(OfficeOwnerInfo info)
        : this()
    {
        FirstName = info.FirstName;
        MiddleName = info.MiddleName;
        LastName = info.LastName;
        Email = info.Email;
        MobilePhone = info.MobilePhone;
        DOB = info.DOB;
        SSN = info.SSN;
        Ownership = info.Ownership;
        AnnualIncome = info.AnnualIncome;
        NetWorth = info.NetWorth;
        Address = info.Address;
    }

    protected OfficeOwner()
    {

    }

    public void UpdateInfo(OfficeOwner info)
    {
        FirstName = info.FirstName;
        LastName = info.LastName;
        MiddleName = info.MiddleName;
        Email = info.Email;
        MobilePhone = info.MobilePhone;
        Ownership = info.Ownership == 0 ? Ownership : info.Ownership;
        AnnualIncome = info.AnnualIncome ?? AnnualIncome;
        NetWorth = info.NetWorth ?? NetWorth;
        DOB = info.DOB;
        SSN = info.SSN;

        if (info.Address is not null)
        {
            Address?.Update(info.Address);

        }
    }

    public readonly record struct OfficeOwnerInfo(
        string FirstName,
        string LastName,
        string? MiddleName,
        string Email,
        string? MobilePhone,
        string? DOB,
        string? SSN,
        short Ownership,
        int? AnnualIncome,
        int? NetWorth,
        Address? Address
    );

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return Email;
        yield return MobilePhone;
        yield return DOB;
        yield return SSN;
    }
}
