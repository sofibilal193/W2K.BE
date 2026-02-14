using DFI.Common.Application.Cacheing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DFI.Common.Application.Behaviors;

/// <summary>
/// MediatR Caching Pipeline Behavior
/// https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class CachePipelineBehavior<TRequest, TResponse>(
    ICache cache,
    ILogger<CachePipelineBehavior<TRequest, TResponse>> logger,
    IEnumerable<ICachePolicy<TRequest, TResponse>> cachePolicies) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<ICachePolicy<TRequest, TResponse>> _cachePolicies = cachePolicies;

    // ICache is a helper wrapper over MemoryCache that adds some read-through cache methods, etc.
    private readonly ICache _cache = cache;
    private readonly ILogger<CachePipelineBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cachePolicy = _cachePolicies.FirstOrDefault();
        if (cachePolicy is null)
        {
            // No cache policy found, so just continue through the pipeline
            return await next(cancellationToken);
        }
        var cacheKey = cachePolicy.GetCacheKey(request);
        TResponse? cachedResponse = await _cache.GetAsync<TResponse>(cachePolicy.AppName, cacheKey, cancellationToken);
        if (cachedResponse is not null && !IsDefaultValue(cachedResponse))
        {
            _logger.LogInformation("Response retrieved {Otype} from cache. CacheKey: {Key}", typeof(TRequest).FullName, cacheKey);
            return cachedResponse;
        }

        var response = await next(cancellationToken);
        if (response is not null)
        {
            _logger.LogInformation("Caching response for {Otype} with cache key: {Key}", typeof(TRequest).FullName, cacheKey);

            var expiration = cachePolicy.SlidingExpiration(request, response);
            if (expiration.HasValue)
            {
                await _cache.SetSlidingAsync(cachePolicy.AppName, cacheKey, response, expiration.Value, cancellationToken);
                return response;
            }
            expiration = cachePolicy.AbsoluteExpirationRelativeToNow(request, response);
            if (expiration.HasValue)
            {
                await _cache.SetAsync(cachePolicy.AppName, cacheKey, response, expiration.Value, cancellationToken);
                return response;
            }
            var expirationOffset = cachePolicy.AbsoluteExpiration(request, response);
            if (expirationOffset.HasValue)
            {
                await _cache.SetAsync(cachePolicy.AppName, cacheKey, response, expirationOffset.Value, cancellationToken);
                return response;
            }
        }
        return response;
    }

    /// <summary>
    /// Checks if a value is the default value for its type.
    /// For struct types, compares with default(T). For reference types, checks for null.
    /// </summary>
    private static bool IsDefaultValue(TResponse value)
    {
        if (value is null)
        {
            return true;
        }

        // For value types (structs), compare with default
        if (typeof(TResponse).IsValueType)
        {
            return EqualityComparer<TResponse>.Default.Equals(value, default!);
        }

        return false;
    }
}
