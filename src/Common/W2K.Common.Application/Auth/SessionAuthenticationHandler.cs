#pragma warning disable CA1508 // Avoid dead conditional code
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using W2K.Common.Application.Session;

namespace W2K.Common.Application.Auth;

public class SessionAuthenticationHandler(
   IOptionsMonitor<AuthenticationSchemeOptions> options,
   ILoggerFactory loggerFactory,
   UrlEncoder encoder,
   ISessionStore sessionStore) : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private readonly ILogger<SessionAuthenticationHandler> _logger = loggerFactory.CreateLogger<SessionAuthenticationHandler>();
    private readonly ISessionStore _sessionStore = sessionStore;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Skip session authentication if an authorization header is present (JWT/Bearer tokens take precedence)
        if (Request.Headers.Authorization.Count > 0)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Skip session authentication, if an API key header is present and the current scheme is not SessionAuthScheme
        if (Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKeyValues) && apiKeyValues.Count > 0
            && !string.IsNullOrWhiteSpace(apiKeyValues[0])
            && !Scheme.Name.Equals(AuthConstants.SessionAuthScheme, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Extract session headers - only check presence, detailed validation happens in SessionRequirementHandler
        if (!TryExtractHeaders(out var sessionId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing or invalid session headers."));
        }

        if (sessionId is not null)
        {
            var isValidSessionId = _sessionStore.IsValidSessionIdAsync(sessionId).GetAwaiter().GetResult();

            if (!isValidSessionId)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid session ID."));
            }

            _logger.LogInformation("Session with Id {SessionId} successfully retrieved.", sessionId);

            // Create session principal with basic claims to bypass permission checks in PermissionAuthorizationHandler
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "SessionUser"),
                new(AuthConstants.SessionIdHeaderName, sessionId)
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        // If sessionId is null, fail authentication
        return Task.FromResult(AuthenticateResult.Fail("Session ID is null."));
    }

    private bool TryExtractHeaders(out string? sessionId)
    {
        sessionId = null;

        if (!Request.Headers.TryGetValue(AuthConstants.SessionIdHeaderName, out var sessionIdValues) || sessionIdValues.Count == 0 || string.IsNullOrWhiteSpace(sessionIdValues[0]))
        {
            _logger.LogInformation("Session auth failed: Missing {Header}", AuthConstants.SessionIdHeaderName);
            return false;
        }

        sessionId = sessionIdValues[0]?.Trim();

        if (sessionId is null)
        {
            _logger.LogInformation("Session auth failed: Null header values");
            return false;
        }

        return !string.IsNullOrEmpty(sessionId);
    }
}
#pragma warning restore CA1508 // Avoid dead conditional code
