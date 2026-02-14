#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using W2K.Common.Enums;

/// <summary>
/// Command to create or update a session log record when a user interacts with sessions.
/// </summary>
public record UpsertSessionLogCommand(
    string? SessionId,
    string? Fingerprint,
    string? OldSessionId = null,
    bool IsLogout = false,
    string? Source = null) : IRequest;
