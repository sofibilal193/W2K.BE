#pragma warning disable CA1716 // Identifiers should not match keywords
namespace DFI.Common.Application.Cacheing;

/// <summary>
/// Defines a contract for an asynchronous and synchronous cache provider supporting various expiration strategies.
/// </summary>
public interface ICache : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Retrieves a cached item by application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>The cached item if found; otherwise, <c>null</c>.</returns>
    T? Get<T>(string appName, string key);

    /// <summary>
    /// Asynchronously retrieves a cached item by application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The cached item if found; otherwise, <c>null</c>.</returns>
    Task<T?> GetAsync<T>(string appName, string key, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously retrieves a list of matching cache items by application name and key pattern.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="keyMatchPattern">A pattern to match cache entry keys.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of cached items if found; otherwise, an empty list.</returns>
    Task<IList<T>> GetAllAsync<T>(string appName, string keyMatchPattern, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously sets a cache entry with the specified application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="item">The item to cache.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task SetAsync<T>(string appName, string key, T item, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously sets a cache entry with an absolute expiration relative to now.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="item">The item to cache.</param>
    /// <param name="absoluteExpirationRelativeToNow">The time span after which the cache entry should expire.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task SetAsync<T>(string appName, string key, T item, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously sets a cache entry with an absolute expiration date and time.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="item">The item to cache.</param>
    /// <param name="absoluteExpiration">The date and time when the cache entry should expire.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task SetAsync<T>(string appName, string key, T item, DateTimeOffset absoluteExpiration, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously sets a cache entry with a sliding expiration.
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cache entry.</param>
    /// <param name="item">The item to cache.</param>
    /// <param name="slidingExpiration">The sliding expiration time span for the cache entry.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task SetSlidingAsync<T>(string appName, string key, T item, TimeSpan slidingExpiration, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously removes a cache entry by application name and key.
    /// </summary>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cache entry to remove.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task RemoveAsync(string appName, string key, CancellationToken cancel = default);

    /// <summary>
    /// Asynchronously removes all cache entries matching a pattern within the specified application name or namespace.
    /// </summary>
    /// <param name="appName">The application name or namespace for the cache entries.</param>
    /// <param name="keyMatchPattern">A pattern to match cache entry keys for removal.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    Task RemoveAllAsync(string appName, string keyMatchPattern, CancellationToken cancel = default);
}
#pragma warning restore CA1716 // Identifiers should not match keywords
