#pragma warning disable CA1716 // Identifiers should not match keywords
namespace W2K.Common.Application.Session;

public interface ISessionStore
{
    UserSession? Get(string sessionId);

    Task<UserSession?> GetAsync(string sessionId, CancellationToken cancel = default);

    Task<List<UserSession>?> GetByUserIdAsync(string userId, CancellationToken cancel = default);

    Task<UserSession?> GetByUserIdAsync(string userId, string sessionId, CancellationToken cancel = default);

    Task SetAsync(UserSession session, CancellationToken cancel = default);

    Task RemoveAsync(string sessionId, CancellationToken cancel = default);

    Task RemoveByUserIdAsync(string userId, string sessionId, CancellationToken cancel = default);

    Task<bool> IsUserValidAsync(string sessionId, string userId, string base64FingerPrint, CancellationToken cancel = default);

    Task<bool> IsValidAsync(string sessionId, string base64FingerPrint, CancellationToken cancel = default);

    Task<bool> IsValidSessionIdAsync(string sessionId, CancellationToken cancel = default);
}
#pragma warning restore CA1716 // Identifiers should not match keywords
