using W2K.Common.Enums;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertSessionLogCommandHandler(
    IIdentityUnitOfWork data,
    ICurrentUser currentUser) : IRequestHandler<UpsertSessionLogCommand>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task Handle(UpsertSessionLogCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var source = request.Source ?? _currentUser.Source;

        // Determine event type based on context
        var eventType = DetermineEventType(userId, request);

        // Always create a new log entry
        SessionLog sessionLog;
        if (string.IsNullOrEmpty(request.SessionId) || string.IsNullOrEmpty(request.Fingerprint))
        {
            // B2C authentication
            sessionLog = new SessionLog(userId, eventType);
        }
        else
        {
            // Full session data available
            sessionLog = new SessionLog(
                userId,
                request.SessionId,
                request.Fingerprint,
                eventType,
                request.OldSessionId);
        }

        // Set the source explicitly before save to ensure it's captured correctly
        sessionLog.SetLogSource(source);

        _data.SessionLogs.Add(sessionLog);
        await _data.SaveChangesAsync(cancellationToken);
    }

    private static SessionEventType DetermineEventType(int? userId, UpsertSessionLogCommand request)
    {
        // If logout flag is set, it's a logout event
        if (request.IsLogout)
        {
            return SessionEventType.Logout;
        }

        // If no user, it's anonymous
        if (!userId.HasValue)
        {
            return SessionEventType.Anonymous;
        }

        // If OldSessionId is present, it's a session refresh
        if (!string.IsNullOrEmpty(request.OldSessionId))
        {
            return SessionEventType.SessionRefresh;
        }

        // If SessionId is present, it's a new session (after sign-in)
        if (!string.IsNullOrEmpty(request.SessionId))
        {
            return SessionEventType.SessionNew;
        }

        // Otherwise it's a sign-in (B2C authentication without session yet)
        return SessionEventType.SignIn;
    }
}
