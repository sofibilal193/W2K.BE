using W2K.Common.Application.Auth;
using W2K.Common.Application.Session;
using W2K.Common.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Clears the current session for the authenticated user (single-device logout).
/// </summary>
public class LogoutSessionCommandHandler(
    ISessionStore sessionStore,
    ICurrentUser currentUser,
    IHttpContextAccessor httpContextAccessor,
    IMediator mediator,
    ILogger<LogoutSessionCommandHandler> logger) : IRequestHandler<LogoutSessionCommand>
{
    private readonly ISessionStore _sessionStore = sessionStore ?? throw new ArgumentNullException(nameof(sessionStore));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly ILogger<LogoutSessionCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(LogoutSessionCommand request, CancellationToken cancellationToken)
    {
        // Validate user is authenticated
        var userId = _currentUser.AuthProviderId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("Logout called for unauthenticated user. No action taken.");
            return;
        }

        // Extract SessionId from request headers
        var sessionId = _httpContextAccessor.HttpContext?.Request.Headers[AuthConstants.SessionIdHeaderName].FirstOrDefault()?.Trim();

        // Validate SessionId is provided
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogWarning("Logout called with missing or empty SessionId for user {UserId}. No action taken.", userId);
            return;
        }

        // Extract fingerprint from request headers
        var fingerprint = _httpContextAccessor.HttpContext?.Request.Headers[AuthConstants.FingerPrintHeaderName].FirstOrDefault()?.Trim();

        // Validate fingerprint is provided
        if (string.IsNullOrWhiteSpace(fingerprint))
        {
            _logger.LogWarning("Logout called with missing or empty fingerprint for user {UserId}. No action taken.", userId);
            return;
        }

        // Retrieve the session to validate fingerprint
        var session = await _sessionStore.GetByUserIdAsync(userId, sessionId, cancellationToken);
        if (IsDefaultOrNullSession(session))
        {
            _logger.LogWarning("Logout called for non-existent session for user {UserId}.", userId);
            return;
        }

        // Validate fingerprint matches the one in the session
        if (!string.Equals(session.Value.Base64FingerPrint, fingerprint, StringComparison.Ordinal))
        {
            _logger.LogWarning("Logout called with mismatched fingerprint for user {UserId}. Potential security issue.", userId);
            return;
        }


        // Remove the current session only (single-device logout)
        await _sessionStore.RemoveByUserIdAsync(userId, sessionId, cancellationToken);

        // Log session logout event
        try
        {
            var logCommand = new UpsertSessionLogCommand(sessionId, fingerprint, IsLogout: true);
            await _mediator.Send(logCommand, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log session logout event for user {UserId}, session {SessionId}.", userId, sessionId);
        }

        _logger.LogInformation("Successfully logged out user {UserId} from session {SessionId}.", userId, sessionId);
    }

    /// <summary>
    /// Determines whether a UserSession is null or a default-initialized struct.
    /// Returns true if the session has no value or if all fields are null/zero (default struct).
    /// </summary>
    private static bool IsDefaultOrNullSession(UserSession? session)
    {
        if (!session.HasValue)
        {
            return true;
        }

        // For value types (structs), compare with default to catch default-initialized instances
        return EqualityComparer<UserSession>.Default.Equals(session.Value, default);
    }
}
