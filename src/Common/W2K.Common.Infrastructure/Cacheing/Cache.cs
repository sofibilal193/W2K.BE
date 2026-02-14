#pragma warning disable CA1724 // Type names should not match namespaces
using System.Collections.Concurrent;
using DFI.Common.Application.Cacheing;
using DFI.Common.Utils;
using Microsoft.Extensions.Caching.Distributed;

namespace DFI.Common.Infrastructure.Cacheing;

public sealed class Cache(IDistributedCache cache) : ICache
{
    private readonly IDistributedCache _cache = cache;
    private readonly ConcurrentDictionary<string, string> _keys = new();

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region ICache Methods

    /// <summary>
    /// Retrieves a cached item by application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>The cached item if found; otherwise, <c>null</c>.</returns>
    public T? Get<T>(string appName, string key)
    {
        T? item = default;
        var i = _cache.Get(GetCacheKey(appName, key));
        if (i is not null)
        {
            try
            {
                item = i.FromByteArray<T>();
            }
            catch
            {
                // ignore
            }
        }
        return item;
    }

    /// <summary>
    /// Asynchronously retrieves a cached item by application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The cached item if found; otherwise, <c>null</c>.</returns>
    public async Task<T?> GetAsync<T>(string appName, string key, CancellationToken cancel = default)
    {
        T? item = default;
        var i = await _cache.GetAsync(GetCacheKey(appName, key), cancel);
        if (i is not null)
        {
            try
            {
                item = i.FromByteArray<T>();
            }
            catch
            {
                // ignore
            }
        }
        return item;
    }

    /// <summary>
    /// Asynchronously retrieves a list of matching cache items by application name and key pattern.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="keyMatchPattern">A pattern to match cache entry keys.</param>
    /// <param name="cancel">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A list of cached items if found; otherwise, an empty list.</returns>
    public async Task<IList<T>> GetAllAsync<T>(string appName, string keyMatchPattern, CancellationToken cancel = default)
    {
        var matchingKeys = _keys.Keys
            .Where(x => x.StartsWith($"{appName}:", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains(keyMatchPattern, StringComparison.InvariantCultureIgnoreCase));
        var tasks = matchingKeys.Select(x => GetAsync<T>(appName, x, cancel));
        var results = await Task.WhenAll(tasks);
        return [.. results.OfType<T>()];
    }

    public async Task SetAsync<T>(string appName, string key, T item, CancellationToken cancel = default)
    {
        AddKey(appName, key);
        await _cache.SetAsync(GetCacheKey(appName, key), item?.ToByteArray() ?? [], cancel);
    }

    public async Task SetAsync<T>(string appName, string key, T item, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancel = default)
    {
        AddKey(appName, key);
        await _cache.SetAsync(
            GetCacheKey(appName, key),
            item?.ToByteArray() ?? [],
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow },
            cancel);
    }

    public async Task SetAsync<T>(string appName, string key, T item, DateTimeOffset absoluteExpiration, CancellationToken cancel = default)
    {
        AddKey(appName, key);
        await _cache.SetAsync(
            GetCacheKey(appName, key),
            item?.ToByteArray() ?? [],
            new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration },
            cancel);
    }

    public async Task SetSlidingAsync<T>(string appName, string key, T item, TimeSpan slidingExpiration, CancellationToken cancel = default)
    {
        AddKey(appName, key);
        await _cache.SetAsync(
            GetCacheKey(appName, key),
            item?.ToByteArray() ?? [],
            new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration },
            cancel);
    }

    public async Task RemoveAsync(string appName, string key, CancellationToken cancel = default)
    {
        RemoveKey(appName, key);
        await _cache.RemoveAsync(GetCacheKey(appName, key), cancel);
    }

    public async Task RemoveAllAsync(string appName, string keyMatchPattern, CancellationToken cancel = default)
    {
        keyMatchPattern = $"{appName}:{keyMatchPattern}";
        var matchingKeys = _keys.Keys
            .Where(x => x.StartsWith($"{appName}:", StringComparison.InvariantCultureIgnoreCase)
                        && x.Contains(keyMatchPattern, StringComparison.InvariantCultureIgnoreCase));
        foreach (var key in matchingKeys)
        {
            await RemoveAsync(appName, key, cancel);
        }
    }

    #endregion

    #region Private Methods

    private static void Dispose(bool disposing)
    {
        if (disposing)
        {
            // dispose resources
        }
    }

    private static ValueTask DisposeAsyncCore()
    {
        // dispose resources
        return ValueTask.CompletedTask;
    }

    private static string GetCacheKey(string appName, string key)
    {
        return key.StartsWith($"{appName}:", StringComparison.InvariantCultureIgnoreCase) ? key : $"{appName}:{key}";
    }

    private void AddKey(string appName, string key)
    {
        _ = _keys.TryAdd(GetCacheKey(appName, key), key);
    }

    private void RemoveKey(string appName, string key)
    {
        _ = _keys.TryRemove(GetCacheKey(appName, key), out _);
    }

    #endregion
}
#pragma warning restore CA1724 // Type names should not match namespaces
