namespace DFI.Common.Application.Settings;

public enum ApiAuthType
{
    JWT = 0,
    AzureAd = 1,
    ClientCredentials = 2
}

public readonly record struct AuthSettings(
    ApiAuthType Type,
    string? Instance,
    string? TenantId,
    string? ClientId,
    string? Domain,
    string? SignUpSignInPolicyId,
    string? ResetPasswordPolicyId,
    string? EditProfilePolicyId,
    string? DevTokenSecretKey,
    string? DevTokenIssuer,
    DevUser? DevUser,
    string? BasicAuthUserName,
    string? BasicAuthPassword,
    string? ApiKey,
    ClientCredentials? ClientCredentials,
    SessionSettings SessionSettings,
    string? InternalServiceAuthKey,
    Dictionary<string, WebHookAuthSetting> WebHookAuthSettings);

public readonly record struct ClientCredentials(
    string? ClientId,
    string? ClientSecret,
    string? Scope,
    string? Authority);

public readonly record struct DevUser(
    string Email,
    string Password,
    string FirstName,
    string LastName);

public readonly record struct SessionSettings(
    string CacheAppName,
    int UserLockThreshold,
    int FingerprintLockThreshold,
    int CompositeLockThreshold,
    int BaseLockSeconds,
    int MaxLockSeconds,
    int UserWindowMinutes,
    int FingerprintWindowMinutes,
    int CompositeWindowMinutes,
    bool EnforceOnAllRequests);


public record WebHookAuthSetting(string ApiKey);
