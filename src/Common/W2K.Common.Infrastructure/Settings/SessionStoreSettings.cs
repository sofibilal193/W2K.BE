namespace W2K.Common.Infrastructure.Settings;

public record SessionStoreSettings
{
    public string CacheAppName { get; init; } = "SessionStore";

    public int ExpirationMinutes { get; init; } = 60;

    public int CacheExpirationMinutes { get; init; } = 90;

    public int MaxConcurrentUserSessions { get; init; } = 3;

    public int RefreshThresholdMinutes { get; init; } = 10;

    public SessionStoreSettings() { }

    public SessionStoreSettings(SessionStoreSettings settings)
    {
        CacheAppName = settings.CacheAppName;
        ExpirationMinutes = settings.ExpirationMinutes;
        CacheExpirationMinutes = settings.CacheExpirationMinutes;
        MaxConcurrentUserSessions = settings.MaxConcurrentUserSessions;
        RefreshThresholdMinutes = settings.RefreshThresholdMinutes;
    }
}
