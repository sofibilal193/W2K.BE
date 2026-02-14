using W2K.Common.Enums;
using W2K.Common.ValueObjects;
using W2K.Identity.Enums;

namespace W2K.Identity.Entities;

public class OfficeBankAccount : BaseEntityValueObject
{
    public BankAccountType Type { get; private set; }

    public string BankName { get; private set; } = string.Empty;

    public string RoutingNumber { get; private set; } = string.Empty;

    public string AccountName { get; private set; } = string.Empty;

    public string AccountNumber { get; private set; } = string.Empty;

    public bool IsPrimary { get; private set; }

    // Constructors
    public OfficeBankAccount(OfficeBankAccountInfo info)
        : this()
    {
        Type = info.Type;
        BankName = info.BankName;
        RoutingNumber = info.RoutingNumber;
        AccountName = info.AccountName;
        AccountNumber = info.AccountNumber;
        IsPrimary = info.IsPrimary;
    }

    protected OfficeBankAccount()
    {

    }

    public void UpdateInfo(OfficeBankAccount info)
    {
        Type = info.Type;
        BankName = info.BankName;
        RoutingNumber = info.RoutingNumber;
        AccountName = info.AccountName;
        AccountNumber = info.AccountNumber;
        IsPrimary = info.IsPrimary;
    }

    public readonly record struct OfficeBankAccountInfo(
        BankAccountType Type,
        string BankName,
        string RoutingNumber,
        string AccountName,
        string AccountNumber,
        bool IsPrimary
    );

    public void SetAsSecondary()
    {
        IsPrimary = false;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return RoutingNumber;
        yield return AccountNumber;
        yield return BankName;
        yield return IsPrimary;
    }
}
