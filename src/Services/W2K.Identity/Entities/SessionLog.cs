using W2K.Common.Entities;
using W2K.Common.Enums;

namespace W2K.Identity.Entities;

/// <summary>
/// Captures session history for users including sign-ins, session refreshes, and anonymous sessions.
/// </summary>
public class SessionLog : BaseEntity
{
    #region Public Properties

    public int? UserId { get; private set; }

    public string SessionId { get; private set; } = string.Empty;

    public string Fingerprint { get; private set; } = string.Empty;

    public SessionEventType EventType { get; private set; }

    public string? OldSessionId { get; private set; }

    #endregion

    #region Constructors

    public SessionLog(
        int? userId,
        SessionEventType eventType = SessionEventType.SignIn)
    {
        UserId = userId;
        EventType = eventType;
    }

    public SessionLog(
        int? userId,
        string sessionId,
        string fingerprint,
        SessionEventType eventType = SessionEventType.SignIn,
        string? oldSessionId = null)
    {
        UserId = userId;
        SessionId = sessionId;
        Fingerprint = fingerprint;
        EventType = eventType;
        OldSessionId = oldSessionId;
    }

    protected SessionLog() { }

    #endregion

    #region Public Methods

    public void SetLogSource(string? source)
    {
        if (!string.IsNullOrEmpty(source))
        {
            SetSource(source);
        }
    }

    #endregion
}
