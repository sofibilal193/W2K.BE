using W2K.Common.Enums;

namespace W2K.Common.Models;

/// <summary>
/// Model containing information about an office's bank account.
/// </summary>
public readonly record struct OfficeBankAccountInfo
{
    public bool IsPrimary { get; init; }

    public BankAccountType Type { get; init; }

    public string BankName { get; init; }

    public string RoutingNumber { get; init; }

    public string AccountName { get; init; }

    public string AccountNumber { get; init; }
}
