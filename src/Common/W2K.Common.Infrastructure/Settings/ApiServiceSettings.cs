#pragma warning disable CA2227 // Collection properties should be read only
namespace W2K.Common.Infrastructure.Settings;

public class ApiServiceSettings
{
    public Dictionary<string, ApiSettings> Apis { get; set; } = [];

    public int DefaultRetryCount { get; set; }

    public int DefaultRetryDelayMs { get; set; }

    public Uri? GetBaseUri(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings is null ? null : new Uri(settings.BaseUrl);
    }

    public int GetRetryCount(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings?.RetryCount ?? DefaultRetryCount;
    }

    public TimeSpan GetRetryDelay(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return TimeSpan.FromMilliseconds(settings?.RetryDelayMs ?? DefaultRetryDelayMs);
    }

    public string? GetUserName(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings?.UserName;
    }

    public string? GetPassword(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings?.Password;
    }

    public string? GetClientSecret(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings?.ClientSecret;
    }

    public string? GetClientId(string key)
    {
        var settings = Apis.GetValueOrDefault(key);
        return settings?.ClientId;
    }
}

public record ApiSettings
{
    public string BaseUrl { get; init; } = "";

    public int? RetryCount { get; init; }

    public int? RetryDelayMs { get; init; }

    public string? UserName { get; init; }

    public string? Password { get; init; }

    public string? ClientSecret { get; init; }

    public string? ClientId { get; init; }
}
#pragma warning restore CA2227 // Collection properties should be read only
