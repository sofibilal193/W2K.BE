namespace W2K.Common.Application.Auth;

public static class AuthPolicies
{
    public const string ApiKey = "ApiKey";

    public const string Basic = "Basic";

    public const string OfficeUser = "OfficeUser";

    public const string TokenOrApiKey = "OfficeUserOrApiKey";

    public const string SuperAdmin = "SuperAdmin";

    public const string Session = "Session";

    public const string TokenOrSession = "OfficeUserOrSession";

    public const string TokenOrSessionOrApiKey = "OfficeUserOrSessionOrApiKey";

    public const string WebHookApiKey = "WebHookApiKey";
}
