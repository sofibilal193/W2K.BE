namespace W2K.Common.Application.AzureAd;

public readonly record struct AzureADUser(
    string Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string? MobilePhone,
    string? JobTitle,
    DateTimeOffset? LastLoginDateTime,
    string? LastLoginIpAddress)
{
    public string FullName => string.Concat(FirstName, " ", LastName).Trim();
}
