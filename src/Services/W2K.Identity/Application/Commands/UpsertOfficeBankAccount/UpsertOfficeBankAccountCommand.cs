using System.Text.Json.Serialization;
using W2K.Common.Crypto;
using W2K.Common.Enums;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record UpsertOfficeBankAccountCommand : IRequest
{
    [JsonIgnore]
    public int OfficeId { get; private set; }

    public required BankAccountType AccountType { get; init; }

    public required string BankName { get; init; }

    public required string AccountName { get; init; }

    [JsonEncrypted<string>]
    public required string RoutingNumber { get; init; }

    [JsonEncrypted<string>]
    public required string AccountNumber { get; init; }

    public required bool IsPrimary { get; init; }

    public void SetOfficeId(int officeId)
    {
        OfficeId = officeId;
    }
}
