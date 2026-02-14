using W2K.Common.Application.Cacheing;
using W2K.Common.Application.Session;
using W2K.Common.Identity;
using W2K.Common.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace W2K.Common.Infrastructure.Session;

#pragma warning disable IDE0046 // Use conditional expression for return

/// <summary>
/// Implements ISessionStore using ICache for session and CSRF token management.
/// </summary>
public class SessionStore(IOptions<SessionStoreSettings> settings, ICache cache, IServiceProvider serviceProvider) : ISessionStore
{
    private const string _sessionKeyPrefix = "Session_{0}";
    private const string _sessionUserKeyPrefix = "Session_{0}_User_{1}";

    private readonly ICache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly SessionStoreSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public UserSession? Get(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return null;
        }
        // Try to get current user
        var currentUser = GetCurrentUser();
        if (!string.IsNullOrWhiteSpace(currentUser?.AuthProviderId))
        {
            return _cache.Get<UserSession>(_settings.CacheAppName, string.Format(_sessionUserKeyPrefix, sessionId, currentUser.AuthProviderId));
        }
        return _cache.Get<UserSession>(_settings.CacheAppName, string.Format(_sessionKeyPrefix, sessionId));
    }

    public async Task<UserSession?> GetAsync(string sessionId, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return null;
        }
        return await _cache.GetAsync<UserSession>(_settings.CacheAppName, string.Format(_sessionKeyPrefix, sessionId), cancel);
    }

    public async Task<List<UserSession>?> GetByUserIdAsync(string userId, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }
        var sessions = await _cache.GetAllAsync<UserSession>(_settings.CacheAppName, string.Format(_sessionUserKeyPrefix, "*", userId), cancel);
        return sessions?.ToList();
    }

    public async Task<UserSession?> GetByUserIdAsync(string userId, string sessionId, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return null;
        }
        return await _cache.GetAsync<UserSession>(_settings.CacheAppName, string.Format(_sessionUserKeyPrefix, sessionId, userId), cancel);
    }

    public async Task SetAsync(UserSession session, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(session.SessionId))
        {
            throw new ArgumentException("Session or SessionId is null");
        }
        if (session.IsExpired)
        {
            throw new ArgumentException("Session is expired");
        }
        if (string.IsNullOrEmpty(session.UserId))
        {
            await _cache.SetAsync(_settings.CacheAppName, string.Format(_sessionKeyPrefix, session.SessionId), session, TimeSpan.FromMinutes(_settings.CacheExpirationMinutes), cancel);
        }
        else
        {
            // Store session by user ID for quick lookup
            await _cache.SetAsync(_settings.CacheAppName, string.Format(_sessionUserKeyPrefix, session.SessionId, session.UserId), session, TimeSpan.FromMinutes(_settings.CacheExpirationMinutes), cancel);
        }
    }

    public async Task RemoveAsync(string sessionId, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return;
        }
        await _cache.RemoveAsync(_settings.CacheAppName, string.Format(_sessionKeyPrefix, sessionId), cancel);
    }

    public async Task RemoveByUserIdAsync(string userId, string sessionId, CancellationToken cancel = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }
        await _cache.RemoveAsync(_settings.CacheAppName, string.Format(_sessionUserKeyPrefix, sessionId, userId), cancel);
    }

    public async Task<bool> IsUserValidAsync(string sessionId, string userId, string base64FingerPrint, CancellationToken cancel = default)
    {
        var session = string.IsNullOrEmpty(userId) ? await GetAsync(sessionId, cancel) : await GetByUserIdAsync(userId, sessionId, cancel);
        if (session is null)
        {
            return false;
        }
        if (session.Value.IsExpired)
        {
            return false;
        }
        // Ensure session is for the correct user and fingerprint
        return string.Equals(session.Value.SessionId, sessionId, StringComparison.Ordinal)
            && string.Equals(session.Value.UserId, userId, StringComparison.Ordinal)
            && string.Equals(session.Value.Base64FingerPrint, base64FingerPrint, StringComparison.Ordinal);
    }

    public async Task<bool> IsValidAsync(string sessionId, string base64FingerPrint, CancellationToken cancel = default)
    {
        var session = await GetAsync(sessionId, cancel);
        if (session is null)
        {
            return false;
        }
        if (session.Value.IsExpired)
        {
            return false;
        }
        // For CSRF: Validate fingerprint (userId may be null for anonymous)
        return string.Equals(session.Value.Base64FingerPrint, base64FingerPrint, StringComparison.Ordinal);
    }

    public async Task<bool> IsValidSessionIdAsync(string sessionId, CancellationToken cancel = default)
    {
        var session = await GetAsync(sessionId, cancel);
        if (session is null)
        {
            return false;
        }

        if (session.Value.IsExpired)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Safely gets the current user from the service provider if within an HTTP request context
    /// </summary>
    private ICurrentUser? GetCurrentUser()
    {
        try
        {
            // Create a scope and try to get ICurrentUser from Service Provider because SessionStore is registered as singleton and ICurrentUser is scoped
            using var scope = _serviceProvider.CreateScope();
            return scope.ServiceProvider.GetService<ICurrentUser>();
        }
        catch
        {
            // If we're not in an HTTP request context, return null
            return null;
        }
    }
}

#pragma warning restore IDE0046 // Use conditional expression for return
