using W2K.Common.Crypto;
using W2K.Common.Enums;
using ProtoBuf;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application.DTOs;
#pragma warning restore IDE0130 // Namespace does not match folder structure

[Serializable]
[ProtoContract]
public record OfficeBankAccountDto
{
    /// <summary>
    /// Bank account type (e.g., Savings, Checking).
    /// </summary>
    [ProtoMember(1)]
    public BankAccountType Type { get; init; }

    /// <summary>
    /// Bank name.
    /// </summary>
    [ProtoMember(2)]
    public string BankName { get; init; } = string.Empty;

    /// <summary>
    /// Routing number.
    /// </summary>
    [ProtoMember(3)]
    [JsonEncrypted<string>]
    public string RoutingNumber { get; init; } = string.Empty;

    /// <summary>
    /// Account name.
    /// </summary>
    [ProtoMember(4)]
    public string AccountName { get; init; } = string.Empty;

    /// <summary>
    /// Bank account number.
    /// </summary>
    [ProtoMember(5)]
    [JsonEncrypted<string>]
    public string AccountNumber { get; init; } = string.Empty;

    /// <summary>
    /// Indicates whether this is the primary bank account.
    /// </summary>
    [ProtoMember(6)]
    public bool IsPrimary { get; init; }
}
