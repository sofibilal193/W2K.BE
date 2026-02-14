using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using W2K.Common.Application.Settings;
using W2K.Identity.Application.Queries;

namespace W2K.Identity.Auth;

public class JwtBearerOptionsSetup(IOptionsMonitor<AppSettings> settings, IServiceProvider serviceProvider) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AppSettings? _settings = settings.CurrentValue;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (!string.IsNullOrEmpty(_settings?.AuthSettings.ClientId)) // Azure AD Client Id specified
        {
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = OnTokenValidated
            };
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    public async Task OnTokenValidated(ResultContext<JwtBearerOptions> context)
    {
        var claims = new List<Claim>();

        var providerId = context?.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
        var userId = context?.Principal?.FindFirst(Common.Application.Identity.IdentityConstants.UserIdClaimTypeName)?.Value;
        var email = context?.Principal?.FindFirst(ClaimTypes.Email)?.Value ?? context?.Principal?.FindFirst("emails")?.Value;

        // if UserId is not present, attempt to get from local db as user might have recently enrolled and claims only get refreshed in B2C if user logs out and logs back in
        if (!string.IsNullOrEmpty(providerId) && (string.IsNullOrEmpty(userId) || userId == "0")
            && !string.IsNullOrEmpty(email))
        {
            // Create a new scope https://andrewlock.net/the-dangers-and-gotchas-of-using-scoped-services-when-configuring-options-in-asp-net-core/
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            if (mediator is not null)
            {
                var user = await mediator.Send(new GetLoginUserInfoQuery
                {
                    ProviderId = providerId,
                    Email = email,
                    Step = "TokenValidated"
                });
                if (user.Id > 0)
                {
                    claims.Add(new Claim(Common.Application.Identity.IdentityConstants.UserIdClaimTypeName, user.Id.ToString()));
                    // Attach accumulated claims to the authenticated identity
                    if (context!.Principal?.Identity is ClaimsIdentity identity)
                    {
                        identity.AddClaims(claims.Where(x => !identity.HasClaim(c => c.Type == x.Type && c.Value == x.Value)));
                    }
                }
            }
        }
    }
}
