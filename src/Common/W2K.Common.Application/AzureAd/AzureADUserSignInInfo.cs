namespace DFI.Common.Application.AzureAd;

public readonly record struct AzureADUserSignInInfo(
    DateTimeOffset? LastLoginDateTime,
    string? LastLoginIpAddress);
