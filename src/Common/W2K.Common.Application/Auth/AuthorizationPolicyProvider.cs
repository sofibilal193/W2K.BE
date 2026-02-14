using W2K.Common.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace W2K.Common.Application.Auth;

public class AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IOptions<AppSettings> appSettings) : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // check if policy with given name exists
        var policy = await base.GetPolicyAsync(policyName);
        if (policy is not null)
        {
            return policy;
        }

        // no policy was found, so create policy that checks office user permissions
        var permissions = policyName.Split(AuthConstants.PermissionSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var builder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissions));
        if (_appSettings.AuthSettings.SessionSettings.EnforceOnAllRequests)
        {
            _ = builder.AddRequirements(new SessionRequirement());
        }

        return builder.Build();
    }
}
