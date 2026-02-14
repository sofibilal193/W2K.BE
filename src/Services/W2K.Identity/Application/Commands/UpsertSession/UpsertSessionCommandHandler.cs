using W2K.Common.Application.Session;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using Microsoft.Extensions.Logging;
using W2K.Common.Infrastructure.Settings;
using W2K.Common.Crypto;
using AutoMapper;
using Microsoft.Extensions.Options;
using W2K.Common.Application.Validations;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertSessionCommandHandler(
    ISessionStore sessionStore,
    ICurrentUser currentUser,
    ILogger<UpsertSessionCommandHandler> logger,
    IOptions<SessionStoreSettings> settings,
    ICryptoProvider crypto,
    IMapper mapper,
    IMediator mediator) : IRequestHandler<UpsertSessionCommand, UserSessionDto?>
{
    private readonly ISessionStore _sessionStore = sessionStore;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly ILogger<UpsertSessionCommandHandler> _logger = logger;
    private readonly SessionStoreSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly ICryptoProvider _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task<UserSessionDto?> Handle(UpsertSessionCommand request, CancellationToken cancellationToken)
    {
        string? userId = _currentUser.IsAuthenticated ? _currentUser.AuthProviderId : null;

        if (!string.IsNullOrEmpty(request.SessionId))
        {
            var existingSession = string.IsNullOrEmpty(userId)
                ? await _sessionStore.GetAsync(request.SessionId, cancellationToken)
                : await _sessionStore.GetByUserIdAsync(userId, request.SessionId, cancellationToken);

            ValidateSession(existingSession, request.SessionId, request.Base64FingerPrint, userId);

            // Return existing session if remaining time exceeds refresh threshold
            if (existingSession!.Value.RemainingMinutes > _settings.RefreshThresholdMinutes)
            {
                _logger.LogInformation(
                    "Session with Id {SessionId} for User {UserId} is valid with {RemainingMinutes:F2} minutes remaining (above {ThresholdMinutes} min threshold).",
                    request.SessionId,
                    userId,
                    existingSession.Value.RemainingMinutes,
                    _settings.RefreshThresholdMinutes);

                return _mapper.Map<UserSessionDto>(existingSession.Value);
            }

            // Session is within refresh threshold - revoke old session and create new one
            _logger.LogInformation(
                "Session with Id {SessionId} for User {UserId} is within refresh threshold ({RemainingMinutes:F2} min remaining). Creating new session.",
                request.SessionId,
                userId,
                existingSession.Value.RemainingMinutes);

            await RemoveSessionAsync(userId, request.SessionId, cancellationToken);
            return await CreateAndStoreSessionAsync(userId, request.Base64FingerPrint, request.ClientEncryptionKeyBase64Encoded, existingSession.Value.SessionId, cancellationToken);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            var existingSessions = await _sessionStore.GetByUserIdAsync(userId, cancellationToken);

            _logger.LogInformation("User {UserId} has {ExistingSessionCount} existing session(s). Maximum allowed: {MaxConcurrentUserSessions}.", userId, existingSessions?.Count, _settings.MaxConcurrentUserSessions);

            if (existingSessions is { Count: > 0 })
            {
                var activeSessions = existingSessions.Where(x => !x.IsExpired).ToList();

                if (activeSessions.Count >= _settings.MaxConcurrentUserSessions)
                {
                    _logger.LogWarning("User {UserId} has reached the maximum number of concurrent sessions ({MaxSessions}).", userId, _settings.MaxConcurrentUserSessions);
                    throw ValidationCodes.GenerateValidationException(
                        propertyName: nameof(request.SessionId),
                        validationCode: ValidationCodes.MaximumConcurrentSessionsReached);
                }

                if (activeSessions.Exists(x => string.Equals(x.Base64FingerPrint, request.Base64FingerPrint, StringComparison.Ordinal)))
                {
                    _logger.LogWarning("User {UserId} already has an active session with the same fingerprint.", userId);
                    throw ValidationCodes.GenerateValidationException(
                        propertyName: nameof(request.Base64FingerPrint),
                        validationCode: ValidationCodes.SessionFingerprintAlreadyExists);
                }
            }
        }

        return await CreateAndStoreSessionAsync(userId, request.Base64FingerPrint, request.ClientEncryptionKeyBase64Encoded, null, cancellationToken);
    }

    private void ValidateSession(UserSession? session, string sessionId, string base64FingerPrint, string? userId)
    {
        // Session not found or default struct returned
        if (session is null || string.IsNullOrEmpty(session.Value.SessionId))
        {
            _logger.LogWarning("Session with Id {SessionId} not found for UserId {UserId}.", sessionId, userId);
            throw ValidationCodes.GenerateValidationException(
                propertyName: nameof(sessionId),
                validationCode: ValidationCodes.SessionNotFound);
        }

        // Fingerprint mismatch
        if (!string.Equals(session.Value.Base64FingerPrint, base64FingerPrint, StringComparison.Ordinal))
        {
            _logger.LogWarning("Session with Id {SessionId} has mismatched fingerprint for UserId {UserId}.", sessionId, userId);
            throw ValidationCodes.GenerateValidationException(
                propertyName: nameof(base64FingerPrint),
                validationCode: ValidationCodes.SessionFingerprintMismatch);
        }

        // For authenticated users, validate session ownership
        if (_currentUser.IsAuthenticated && !string.Equals(session.Value.UserId, _currentUser.AuthProviderId, StringComparison.Ordinal))
        {
            _logger.LogWarning("Session with Id {SessionId} does not belong to UserId {UserId}.", sessionId, _currentUser.AuthProviderId);
            throw ValidationCodes.GenerateValidationException(
                propertyName: nameof(sessionId),
                validationCode: ValidationCodes.SessionNotOwnedByUser);
        }
    }

    private async Task<UserSessionDto> CreateAndStoreSessionAsync(
        string? userId,
        string base64FingerPrint,
        string? clientEncryptionKeyBase64Encoded,
        string? previousSessionId,
        CancellationToken cancel)
    {
        var session = CreateSession(userId, base64FingerPrint, clientEncryptionKeyBase64Encoded);
        await _sessionStore.SetAsync(session, cancel);
        await CreateSignInLogAsync(session.SessionId, base64FingerPrint, previousSessionId, cancel);
        return _mapper.Map<UserSessionDto>(session);
    }

    private async Task RemoveSessionAsync(string? userId, string sessionId, CancellationToken cancel)
    {
        _logger.LogInformation("Removing Session with Id {SessionId} for User {UserId}.", sessionId, userId);
        if (string.IsNullOrEmpty(userId))
        {
            await _sessionStore.RemoveAsync(sessionId, cancel);
        }
        else
        {
            await _sessionStore.RemoveByUserIdAsync(userId, sessionId, cancel);
        }
    }

    private UserSession CreateSession(string? userId, string base64FingerPrint, string? clientEncryptionKeyBase64Encoded)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_settings.ExpirationMinutes);
        var serverKey = _crypto.GenerateAsymmetricKeyPair();
        return new UserSession
        {
            SessionId = Guid.NewGuid().ToString("N"),
            UserId = userId,
            Base64FingerPrint = base64FingerPrint,
            IssuedAtUtc = now,
            ExpiresAtUtc = expiresAt,
            ClientEncryptionKeyBase64Encoded = clientEncryptionKeyBase64Encoded,
            ServerEncryptionKeyBase64Encoded = serverKey.PublicKeyBase64Encoded,
            ServerDecryptionKeyBase64Encoded = serverKey.PrivateKeyBase64Encoded
        };
    }

    private async Task CreateSignInLogAsync(
        string sessionId,
        string fingerprint,
        string? oldSessionId,
        CancellationToken cancel)
    {
        var command = new UpsertSessionLogCommand(sessionId, fingerprint, oldSessionId);
        await _mediator.Send(command, cancel);
    }
}
