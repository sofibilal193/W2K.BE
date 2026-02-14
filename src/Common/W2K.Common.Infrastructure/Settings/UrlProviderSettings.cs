namespace DFI.Common.Infrastructure.Settings;

public record UrlProviderSettings
{
    public string CacheAppName { get; init; } = string.Empty;

    public string BaseShortUrl { get; init; } = string.Empty;

    public int ShortUrlExpirationHours { get; init; } = 72; // default to 72 hours

    public int ResolveRateLimitPerMinute { get; init; } = 25; // allow 25 resolves per IP per minute

    public IList<string> AllowedHosts { get; init; } = [];

    public UrlProviderSettings() { }

    public UrlProviderSettings(UrlProviderSettings settings)
    {
        CacheAppName = settings.CacheAppName;
        BaseShortUrl = settings.BaseShortUrl;
        ShortUrlExpirationHours = settings.ShortUrlExpirationHours;
        ResolveRateLimitPerMinute = settings.ResolveRateLimitPerMinute;
        AllowedHosts = settings.AllowedHosts;
    }
}
