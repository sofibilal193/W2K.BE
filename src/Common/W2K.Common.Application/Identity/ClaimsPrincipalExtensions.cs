using System.Collections.ObjectModel;
using System.Security.Claims;
using DFI.Common.Application.Auth;
using DFI.Common.Identity;
using DFI.Common.Utils;

namespace DFI.Common.Application.Identity;

public static class ClaimsPrincipalExtensions
{
    public static OfficeType? GetOfficeType(this ClaimsPrincipal user)
    {
        return Enum.TryParse<OfficeType>(user.FindFirst(IdentityConstants.OfficeTypeClaimTypeName)?.Value, out var orgType) ? orgType : null;
    }

    public static bool IsSuperAdmin(this ClaimsPrincipal user)
    {
        return bool.TryParse(user.FindFirst(IdentityConstants.SuperAdminClaimTypeName)?.Value, out var value) && value;
    }

    public static ReadOnlyCollection<int>? GetOfficeIds(this ClaimsPrincipal user)
    {
        var officeIds = user.FindFirst(IdentityConstants.OfficeIdsClaimTypeName)?.Value;
        return string.IsNullOrEmpty(officeIds)
            ? null
            : officeIds
                .Split('|', StringSplitOptions.TrimEntries)
                .ToList()
                .ConvertAll(Convert.ToInt32)
                .AsReadOnly();
    }

    public static bool IsMemberOfOffice(this ClaimsPrincipal user, int officeId)
    {
        return user.GetOfficeIds()?.Contains(officeId) == true;
    }

    public static int? GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(IdentityConstants.UserIdClaimTypeName)?.Value?.ToNullableInt();
    }

    public static bool ContainsOfficeIdClaim(this ClaimsPrincipal user)
    {
        return !string.IsNullOrEmpty(user.FindFirst(IdentityConstants.OfficeIdsClaimTypeName)?.Value);
    }

    public static bool ContainsUserIdClaim(this ClaimsPrincipal user)
    {
        return !string.IsNullOrEmpty(user.FindFirst(IdentityConstants.UserIdClaimTypeName)?.Value);
    }

    public static int? GetDefaultOfficeId(this ClaimsPrincipal user)
    {
        var officeIdValue = user.FindFirst(IdentityConstants.DefaultOfficeIdClaimTypeName)?.Value;
        return int.TryParse(officeIdValue, out var officeId) ? officeId : default;
    }

    public static bool IsClientCredentialsToken(this ClaimsPrincipal user)
    {
        // Heuristic based on observed token shapes:
        // User (auth code) token: has given_name/family_name/emails/name, scope like "default"
        // Client credentials token: scope starts with "app." (e.g. app.default) and lacks user identity claims.
        var scope = user.FindFirst(IdentityConstants.ScopeClaimTypeName)?.Value; // B2C scope claim
        var azpacr = user.FindFirst("azpacr")?.Value; // Azure AD scope claim
        var hasUserIdentityClaims = user.HasClaim(x => x.Type is "given_name" or "family_name" or "emails" or "name");
        return azpacr == "1" && scope?.StartsWith("app.", StringComparison.OrdinalIgnoreCase) == true && !hasUserIdentityClaims;
    }

    public static bool IsSessionAuthenticated(this ClaimsPrincipal user)
    {
        // Check if user has the session authentication type
        var isSessionAuthType = user.Identity?.AuthenticationType == AuthConstants.SessionAuthScheme;

        // Also check for the specific claims set by SessionAuthenticationHandler for additional validation
        var hasSessionUserName = user.HasClaim(ClaimTypes.Name, "SessionUser");
        var sessionIdClaim = user.FindFirst(AuthConstants.SessionIdHeaderName);
        var hasSessionIdClaim = sessionIdClaim is not null && !string.IsNullOrWhiteSpace(sessionIdClaim.Value);

        return isSessionAuthType && hasSessionUserName && hasSessionIdClaim;
    }

    public static bool IsApiKeyAuthenticated(this ClaimsPrincipal user)
    {
        // Check if user has the API key authentication type
        var isApiKeyAuthType = user.Identity?.AuthenticationType == AuthConstants.ApiKeyAuthScheme;

        // Also check for the specific claims set by ApiKeyAuthenticationHandler for additional validation
        var hasApiClientName = user.HasClaim(ClaimTypes.Name, AuthConstants.ApiClientClaimName);

        return isApiKeyAuthType && hasApiClientName;
    }
}
