namespace W2K.Common.Infrastructure.Messaging;

/// <summary>
/// Exception thrown when a short URL key exists logically but has expired (e.g. underlying cache entry TTL elapsed).
/// Introduced to allow API layer to differentiate between an unknown key (404) and an expired key (410 Gone).
/// </summary>
public sealed class ShortUrlExpiredException : Exception
{
    public ShortUrlExpiredException() : base("Short URL has expired.") { }

    public ShortUrlExpiredException(string key) : base($"Short URL key '{key}' has expired.") { }

    public ShortUrlExpiredException(string key, Exception inner) : base($"Short URL key '{key}' has expired.", inner) { }
}
