#pragma warning disable CA1508 // Avoid dead conditional code
using System.Security.Claims;
using W2K.Common.Application.Session;
using W2K.Common.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using W2K.Common.Application.Identity;
using W2K.Common.Crypto;

namespace W2K.Common.Application.Auth;

/// <summary>
/// Handles validation of session headers and ensures the session is valid.
/// For authenticated requests, binds session to the current user id and fingerprint.
/// For anonymous requests, validates session id + fingerprint only.
/// </summary>
public class SessionRequirementHandler(
    ISessionStore sessionStore,
    ICurrentUser currentUser,
    ILogger<SessionRequirementHandler> logger,
    IHttpContextAccessor httpContextAccessor,
    IThrottlingStore throttlingStore,
    ICryptoProvider cryptoProvider) : AuthorizationHandler<SessionRequirement>
{

    // Instance fields
    private readonly ISessionStore _sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly ILogger<SessionRequirementHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IThrottlingStore _throttlingStore = throttlingStore ?? throw new ArgumentNullException(nameof(throttlingStore));
    private readonly ICryptoProvider _cryptoProvider = cryptoProvider ?? throw new ArgumentNullException(nameof(cryptoProvider));

    #region Core Handling

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            _logger.LogWarning("Session auth failed: No HttpContext available.");
            return; // cannot succeed
        }
        // Client credentials (app-to-app) tokens bypass session enforcement
        if (_currentUser.IsClientCredentialsToken)
        {
            _logger.LogInformation("Session auth bypass: client credentials token.");
            context.Succeed(requirement);
            return;
        }

        // API Key authentication bypasses session enforcement
        if (context.User.Identity?.AuthenticationType == AuthConstants.ApiKeyAuthScheme
            || context.User.HasClaim(ClaimTypes.Name, AuthConstants.ApiClientClaimName))
        {
            _logger.LogInformation("Session auth bypass: API Key authentication.");
            context.Succeed(requirement);
            return;
        }

        if (!TryExtractHeaders(httpContext, out var sessionId, out var fingerPrint))
        {
            context.Fail(new AuthorizationFailureReason(this, "Session headers are missing or invalid"));
            return;
        }
        string? userKey = null;
        if (_currentUser.IsAuthenticated && !string.IsNullOrEmpty(_currentUser.AuthProviderId))
        {
            userKey = GetUserKey(_currentUser.AuthProviderId);
        }
        var fpKey = GetFingerprintKey(fingerPrint);
        var compositeKey = GetCompositeKey(httpContext, fingerPrint);

        var throttleContext = new ThrottlingContext(userKey, fpKey, compositeKey);

        var locked = await _throttlingStore.IsLockedAsync(throttleContext, httpContext.RequestAborted);
        if (locked)
        {
            _logger.LogWarning("Session auth throttled: Partition lock active. User={UserId} SessionId={SessionId} FingerPrintHash={FingerHash}", _currentUser.UserId, sessionId, ShortHash(fingerPrint));
            context.Fail(new AuthorizationFailureReason(this, "Session validation throttled."));
            return;
        }

        var valid = await ValidateSessionAsync(sessionId, fingerPrint, httpContext.RequestAborted);
        if (valid)
        {
            await _throttlingStore.ClearAsync(throttleContext, httpContext.RequestAborted);
            context.Succeed(requirement);
        }
        else
        {
            await _throttlingStore.RegisterFailureAsync(throttleContext, httpContext.RequestAborted);
            context.Fail(new AuthorizationFailureReason(this, "Session validation failed."));
        }
    }

    private bool TryExtractHeaders(HttpContext httpContext, out string sessionId, out string fingerPrint)
    {
        sessionId = string.Empty;
        fingerPrint = string.Empty;
        var headers = httpContext.Request.Headers;

        if (!headers.TryGetValue(AuthConstants.SessionIdHeaderName, out var sessionIdValues) || sessionIdValues.Count == 0 || string.IsNullOrWhiteSpace(sessionIdValues[0]))
        {
            _logger.LogInformation("Session auth failed: Missing {Header}", AuthConstants.SessionIdHeaderName);
            return false;
        }
        if (!headers.TryGetValue(AuthConstants.FingerPrintHeaderName, out var fingerprintValues) || fingerprintValues.Count == 0 || string.IsNullOrWhiteSpace(fingerprintValues[0]))
        {
            _logger.LogInformation("Session auth failed: Missing {Header}", AuthConstants.FingerPrintHeaderName);
            return false;
        }

        var sessionIdRaw = sessionIdValues[0];
        var fingerPrintRaw = fingerprintValues[0];
        if (sessionIdRaw is null || fingerPrintRaw is null)
        {
            _logger.LogInformation("Session auth failed: Null header values");
            return false;
        }
        sessionId = sessionIdRaw.Trim();
        fingerPrint = fingerPrintRaw.Trim();
        return true;
    }

    private async Task<bool> ValidateSessionAsync(string sessionId, string fingerPrint, CancellationToken cancel)
    {
        try
        {
            if (_currentUser.IsAuthenticated && !string.IsNullOrEmpty(_currentUser.AuthProviderId))
            {
                var validUser = await _sessionStore.IsUserValidAsync(sessionId, _currentUser.AuthProviderId, fingerPrint, cancel);
                if (!validUser)
                {
                    _logger.LogWarning("Session auth failed: Invalid user-bound session. UserId={UserId} SessionId={SessionId} FingerPrint={FingerPrint}", _currentUser.UserId, sessionId, fingerPrint);
                }
                return validUser;
            }

            var validAnon = await _sessionStore.IsValidAsync(sessionId, fingerPrint, cancel);
            if (!validAnon)
            {
                _logger.LogWarning("Session auth failed: Invalid anonymous session. SessionId={SessionId} FingerPrint={FingerPrint}", sessionId, fingerPrint);
            }
            return validAnon;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Session auth exception: {Message}", ex.Message);
            return false;
        }
    }

    #endregion

    #region Throttling Helper Key Builders
    private static string GetUserKey(string authProviderId)
    {
        return $"Session_User_Throttling:{authProviderId}";
    }

    private string GetFingerprintKey(string fp)
    {
        return $"Session_Fingerprint_Throttling:{ShortHash(fp)}";
    }

    private string GetCompositeKey(HttpContext ctx, string fp)
    {
        var ua = ctx.Request.Headers.UserAgent.ToString();
        var acceptLang = ctx.Request.Headers.AcceptLanguage.ToString();
        var ip = ctx.GetRequestIp();
        var raw = string.Join('|', ip, ua, acceptLang, ShortHash(fp));
        return $"Session_Composite_Throttling:{ShortHash(raw)}";
    }

    private string ShortHash(string input)
    {
        return _cryptoProvider.ShortHash(input) ?? "ERRHASH";
    }
    #endregion
}
#pragma warning restore CA1508 // Avoid dead conditional code
