using System.Security.Claims;
using System.Text;
using DFI.Common.Application.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DFI.Common.Application.Auth;

public class JwtBearerOptionsSetup(IOptions<AppSettings> settings) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AppSettings? _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (_settings?.AuthSettings.Type == ApiAuthType.JWT && !string.IsNullOrEmpty(_settings.AuthSettings.DevTokenSecretKey))
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settings.AuthSettings.DevTokenIssuer,
                ValidAudience = _settings.AuthSettings.DevTokenIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AuthSettings.DevTokenSecretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = x =>
                {
                    if (x.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        x.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        }
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    public static Task OnTokenValidated(ResultContext<JwtBearerOptions> context)
    {
        var claims = new List<Claim>();

        // add permissions as role claims
        var perms = context.Principal?.FindFirst("extension_OfficePermissions")?.Value;
        if (!string.IsNullOrEmpty(perms))
        {
            claims.AddRange(perms.Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new Claim(ClaimTypes.Role, x)));
        }

        // add role as role claim
        var role = context.Principal?.FindFirst("extension_OfficeRole")?.Value;
        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (claims.Count > 0)
        {
            var appIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            context.Principal?.AddIdentity(appIdentity);
        }

        return Task.CompletedTask;
    }
}
