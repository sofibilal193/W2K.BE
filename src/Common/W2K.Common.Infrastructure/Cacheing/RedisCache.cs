#pragma warning disable CA1849 // Call async methods when in an async method
using DFI.Common.Application.Cacheing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DFI.Common.Infrastructure.Cacheing;

public class RedisCache(IDistributedCache cache, IHostEnvironment env, IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCache> logger) : ICache
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IHostEnvironment _env = env ?? throw new ArgumentNullException(nameof(env));
    private readonly ILogger<RedisCache> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private bool _disposed;
    private readonly IConnectionMultiplexer _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        await Task.CompletedTask;
    }

    #endregion

    /// <summary>
    /// Retrieves a cached item by application name and key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="appName">The application name or namespace for the cache entry.</param>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>The cached item if found; otherwise, <c>null</c>.</returns>
    public T? Get<T>(string appName, string key)
    {
        var redisKey = GetRedisKey(appName, key);
        var value = _cache.Get(redisKey);
        return value is null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
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
        var redisKey = GetRedisKey(appName, key);

        _logger.LogInformation("Retrieving cache item with key {Key} for application {AppName} forming redis key {RedisKey}", key, appName, redisKey);

        var value = await _cache.GetAsync(redisKey, cancel);
        return value is null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
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
        var pattern = GetRedisKey(appName, keyMatchPattern);

        _logger.LogInformation("Retrieving all cache items with pattern {KeyMatchPattern} for application {AppName} forming redis key pattern {Pattern}", keyMatchPattern, appName, pattern);

        var resultList = new List<T>();
        var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints()[0]);

        // use pageSize to limit SCAN iterations and avoid timeout
        await foreach (var redisKey in server.KeysAsync(pattern: pattern, pageSize: 250).WithCancellation(cancel))
        {
            var item = await GetAsync<T>(appName, redisKey.ToString(), cancel);
            if (item is not null)
            {
                resultList.Add(item);
            }
        }

        _logger.LogInformation("Retrieved {Count} cache items with pattern {KeyMatchPattern} for application {AppName}", resultList.Count, keyMatchPattern, appName);
        return resultList;
    }

    public async Task SetAsync<T>(string appName, string key, T item, CancellationToken cancel = default)
    {
        var redisKey = GetRedisKey(appName, key);
        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item);
        await _cache.SetAsync(redisKey, bytes, new DistributedCacheEntryOptions(), cancel);
    }

    public async Task SetAsync<T>(string appName, string key, T item, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancel = default)
    {
        var redisKey = GetRedisKey(appName, key);
        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item);
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow };
        await _cache.SetAsync(redisKey, bytes, options, cancel);
    }

    public async Task SetAsync<T>(string appName, string key, T item, DateTimeOffset absoluteExpiration, CancellationToken cancel = default)
    {
        var redisKey = GetRedisKey(appName, key);
        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item);
        await _cache.SetAsync(redisKey, bytes, new DistributedCacheEntryOptions { AbsoluteExpiration = absoluteExpiration }, cancel);
    }

    public async Task SetSlidingAsync<T>(string appName, string key, T item, TimeSpan slidingExpiration, CancellationToken cancel = default)
    {
        var redisKey = GetRedisKey(appName, key);
        var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(item);
        var options = new DistributedCacheEntryOptions { SlidingExpiration = slidingExpiration };
        await _cache.SetAsync(redisKey, bytes, options, cancel);
    }

    public async Task RemoveAsync(string appName, string key, CancellationToken cancel = default)
    {
        var redisKey = GetRedisKey(appName, key);
        await _cache.RemoveAsync(redisKey, cancel);
    }

    public async Task RemoveAllAsync(string appName, string keyMatchPattern, CancellationToken cancel = default)
    {
        var removedCount = 0;
        var pattern = GetRedisKey(appName, keyMatchPattern);
        _logger.LogInformation("Removing all cache items with pattern {KeyMatchPattern} for application {AppName} with redis key pattern {Pattern}", keyMatchPattern, appName, pattern);
        foreach (var server in _connectionMultiplexer.GetServers())
        {
            // use pageSize to limit SCAN iterations and avoid timeout
            await foreach (var key in server.KeysAsync(pattern: pattern, pageSize: 250).WithCancellation(cancel))
            {
                await _cache.RemoveAsync(key.ToString(), cancel);
                removedCount++;
            }
        }
        _logger.LogInformation("Removed {Count} cache items with pattern {KeyMatchPattern} for application {AppName}", removedCount, keyMatchPattern, appName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _connectionMultiplexer?.Dispose();
            _disposed = true;
        }
    }

    private string GetRedisKey(string appName, string key)
    {
        return key.StartsWith($"{_env.EnvironmentName}:{appName}:", StringComparison.OrdinalIgnoreCase) ? key : $"{_env.EnvironmentName}:{appName}:{key}";
    }
}
#pragma warning restore CA1849 // Call async methods when in an async method
