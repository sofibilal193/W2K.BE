using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Web;
using DFI.Common.Application.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Application.Auth;

public class ApiKeyAuthenticationHandler(
   IOptionsMonitor<AuthenticationSchemeOptions> options,
   ILoggerFactory loggerFactory,
   UrlEncoder encoder,
   IOptions<AppSettings> settingsOptions) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly AppSettings _settings = settingsOptions.Value;
    private readonly ILogger<ApiKeyAuthenticationHandler> _logger = loggerFactory.CreateLogger<ApiKeyAuthenticationHandler>();

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // check if authorization header is present and if so, skip api-key auth
        if (Request.Headers.Authorization.Count > 0)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            var queryParameters = HttpUtility.ParseQueryString(Request.QueryString.ToString());
            extractedApiKey = queryParameters.GetValues(AuthConstants.ApiKeyHeaderName)?.FirstOrDefault();
            if (string.IsNullOrEmpty(extractedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("Api Key was not provided in header or query string of request."));
            }
        }

        var apiKey = _settings.AuthSettings.ApiKey;
        if (string.IsNullOrEmpty(apiKey))
        {
            _logger.LogError("Settings.AuthSettings.ApiKey is missing.");
            return Task.FromResult(AuthenticateResult.Fail("Settings.AuthSettings.ApiKey is missing."));
        }

        if (apiKey.Equals(extractedApiKey, StringComparison.Ordinal))
        {
            var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, AuthConstants.ApiClientClaimName)
                };
            // Try officeId from route values first; if not present, fall back to query string
            if (Request.RouteValues.TryGetValue(AuthConstants.OfficeIdRouteValue, out var value)
                && int.TryParse(value?.ToString(), out var officeId))
            {
                claims.Add(new Claim(AuthConstants.ApiKeyOfficeIdClaimName, officeId.ToString()));
            }
            else
            {
                var queryParameters = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                var officeIdFromQuery = queryParameters.GetValues(AuthConstants.OfficeIdRouteValue)?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(officeIdFromQuery)
                    && int.TryParse(officeIdFromQuery, out var officeIdFromQueryParsed))
                {
                    claims.Add(new Claim(AuthConstants.ApiKeyOfficeIdClaimName, officeIdFromQueryParsed.ToString()));
                }
            }
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        return Task.FromResult(AuthenticateResult.Fail("Api Key is not valid"));
    }
}
