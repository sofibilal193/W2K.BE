using System.Security.Claims;
using System.Text.Encodings.Web;
using W2K.Common.Application.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Common.Application.Auth;

public class WebHookApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IOptions<AppSettings> settingsOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private const string LenderCodeRouteValue = "lenderCode";
    private readonly AppSettings _settings = settingsOptions.Value;
    private readonly ILogger<WebHookApiKeyAuthenticationHandler> _logger = loggerFactory.CreateLogger<WebHookApiKeyAuthenticationHandler>();

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Extract the API key from Authorization header
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            _logger.LogWarning("Authorization header is missing");
            return Task.FromResult(AuthenticateResult.Fail("Authorization header is missing."));
        }

        var authValue = authHeader.ToString().Trim();
        var apiKeyPrefix = $"{AuthConstants.WebhookApiAuthScheme}";

        if (string.IsNullOrWhiteSpace(authValue) || !authValue.StartsWith(apiKeyPrefix, StringComparison.Ordinal))
        {
            _logger.LogWarning("Authorization header must use format '{Scheme} <api-key>'", AuthConstants.WebhookApiAuthScheme);
            return Task.FromResult(AuthenticateResult.Fail($"Authorization header must use format '{AuthConstants.WebhookApiAuthScheme} <api-key>'"));
        }

        var apiKeyHeader = authValue[apiKeyPrefix.Length..].Trim();

        if (string.IsNullOrWhiteSpace(apiKeyHeader))
        {
            _logger.LogWarning("API key is missing in Authorization header.");
            return Task.FromResult(AuthenticateResult.Fail("API key is missing in Authorization header."));
        }

        // Extract lender code from route values
        if (!Request.RouteValues.TryGetValue(LenderCodeRouteValue, out var lenderCode))
        {
            _logger.LogWarning("Lender code is missing in route.");
            return Task.FromResult(AuthenticateResult.Fail("Lender code is missing in route."));
        }

        // Get WebHook auth settings
        var webHookAuthSettings = _settings.AuthSettings.WebHookAuthSettings;
        if (webHookAuthSettings is null)
        {
            _logger.LogWarning("Lender settings are not configured.");
            return Task.FromResult(AuthenticateResult.Fail("Lender settings are not configured."));
        }

        // Fetch API key from AppSettings from lender code (case insensitive)
        var lenderSettings = webHookAuthSettings.FirstOrDefault(x => string.Equals(x.Key, lenderCode?.ToString(), StringComparison.OrdinalIgnoreCase)).Value;

        if (lenderSettings is null || string.IsNullOrWhiteSpace(lenderSettings.ApiKey))
        {
            _logger.LogWarning("Lender not recognized: {LenderCode}", lenderCode);
            return Task.FromResult(AuthenticateResult.Fail("Lender not recognized."));
        }

        // Compare keys
        if (!lenderSettings.ApiKey.Equals(apiKeyHeader.ToString(), StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid API key: {ApiKey} for lender: {LenderCode}", apiKeyHeader, lenderCode);
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        // Build claims & succeed
        var claims = new List<Claim>
        {
             new("auth_scheme", "webhook-apikey"),
            new(ClaimTypes.Name, $"{lenderCode}")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        _logger.LogInformation("Successfully authenticated lender {LenderCode} with API key.", lenderCode);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
