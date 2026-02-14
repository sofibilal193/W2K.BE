using DFI.Common.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace DFI.Common.Application.Auth;

public class MicrosoftIdentityOptionsSetup(IOptions<AppSettings> settings) : IConfigureNamedOptions<MicrosoftIdentityOptions>
{
    private readonly AppSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

    public void Configure(string? name, MicrosoftIdentityOptions options)
    {
        // check if Azure AD B2C is configured
        if (_settings?.AuthSettings.Type == ApiAuthType.AzureAd
            && !string.IsNullOrEmpty(_settings.AuthSettings.Instance)
            && !string.IsNullOrEmpty(_settings.AuthSettings.ClientId)
            && !string.IsNullOrEmpty(_settings.AuthSettings.Domain))
        {
            options.Instance = _settings.AuthSettings.Instance;
            options.ClientId = _settings.AuthSettings.ClientId;
            options.Domain = _settings.AuthSettings.Domain;
            options.SignUpSignInPolicyId = _settings.AuthSettings.SignUpSignInPolicyId;
            options.ResetPasswordPolicyId = _settings.AuthSettings.ResetPasswordPolicyId;
            options.EditProfilePolicyId = _settings.AuthSettings.EditProfilePolicyId;
            options.TokenValidationParameters.NameClaimType = "name";
        }
        else if (_settings?.AuthSettings.Type == ApiAuthType.AzureAd
            && !string.IsNullOrEmpty(_settings.AuthSettings.Instance)
            && !string.IsNullOrEmpty(_settings.AuthSettings.ClientId)
            && !string.IsNullOrEmpty(_settings.AuthSettings.TenantId))
        {
            options.Instance = _settings.AuthSettings.Instance;
            options.ClientId = _settings.AuthSettings.ClientId;
            options.TenantId = _settings.AuthSettings.TenantId;
        }
    }

    public void Configure(MicrosoftIdentityOptions options)
    {
        Configure(Options.DefaultName, options);
    }
}
