namespace W2K.Common.Application.Auth;

public static class AuthConstants
{
    // Allowed methods for actual requests. Do NOT include "OPTIONS"; CORS middleware handles preflight.
    // Limit methods to common CRUD verbs actually used. Expand cautiously.
    public static readonly string[] AllowedCorsMethods = ["GET", "POST", "PUT", "PATCH", "DELETE"];

    public static readonly string BasicAuthenticationScheme = "Basic";

    public static readonly string ApiKeyHeaderName = "X-API-Key";

    public static readonly string OfficeIdRouteValue = "officeId";

    public static readonly string OfficeAccessRouteValue = "officeAccess";

    public static readonly string[] ValidateOfficeUserKeyValues = ["ValidateOfficeUser", "GetUserPermissions"];

    public static readonly string ApiKeyOfficeIdClaimName = "extension_OfficeIds";

    public static readonly string ApiKeyAuthScheme = "ApiKey";

    public static readonly string ApiClientClaimName = "ApiClient";

    public static readonly string SessionAuthScheme = "Session";

    public static readonly string BasicAuthScheme = "Basic";

    public static readonly string BearerAuthScheme = "Bearer";

    public static readonly string PermissionSeparator = ";";

    // Session / Fingerprint headers
    public static readonly string SessionIdHeaderName = "X-Session-Id";

    public static readonly string AppInsightsSessionIdHeaderName = "Session-Id";

    public static readonly string FingerPrintHeaderName = "X-FingerPrint";

    public static readonly string XForwardedForHeaderName = "X-Forwarded-For";

    public static readonly string RemoteAddressHeaderName = "REMOTE_ADDR";

    public static readonly string ClientIpHeaderName = "X-Client-IP";

    public static readonly string AzureClientIpHeaderName = "X-Azure-ClientIP";

    public static readonly string InternalServiceAuthKeyHeaderName = "X-Internal-Key";

    public static readonly string WebhookApiAuthScheme = "WebHookApiKey";
}
