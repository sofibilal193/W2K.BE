using System.Text.Json.Serialization;
using W2K.Common.Application.Commands;
using W2K.Common.Crypto;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public record UpsertOfficeOwnerCommand : IRequest
{
    [JsonIgnore]
    public int OfficeId { get; private set; }

    public IEnumerable<OfficeOwnerInfoCommand> Owners { get; init; } = [];

    public void SetOfficeId(int officeId)
    {
        OfficeId = officeId;
    }
}

public record OfficeOwnerInfoCommand
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public string? MiddleName { get; init; }

    [JsonEncrypted<string>]
    public required string Email { get; init; }

    [JsonEncrypted<string>]
    public required string MobilePhone { get; init; }

    [JsonEncrypted<string>]
    public required string DOB { get; init; }

    [JsonEncrypted<string>]
    public required string SSN { get; init; }

    public int? AnnualIncome { get; init; }

    public int? NetWorth { get; init; }

    public required short Ownership { get; init; }

    public AddressCommand Address { get; init; } = new();
}


