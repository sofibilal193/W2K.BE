using DFI.Common.Application.Auth;
using DFI.Common.Application.Cacheing;
using DFI.Common.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProtoBuf;

namespace DFI.Common.Infrastructure.Auth;

/// <summary>
/// Default implementation of <see cref="IThrottlingStore"/> backed by <see cref="ICache"/>.
/// Mirrors previous logic that lived inside SessionRequirementHandler.
/// </summary>
public sealed class ThrottlingStore(ICache cache, IOptions<AppSettings> settings, ILogger<ThrottlingStore> logger) : IThrottlingStore
{
    private readonly ICache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly AppSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly ILogger<ThrottlingStore> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<bool> IsLockedAsync(ThrottlingContext context, CancellationToken cancel = default)
    {
        var sessionCfg = _settings.AuthSettings.SessionSettings;
        if (context.UserKey is not null && await IsPartitionLockedAsync(context.UserKey, sessionCfg.CacheAppName, cancel))
        {
            return true;
        }

        if (await IsPartitionLockedAsync(context.FingerprintKey, sessionCfg.CacheAppName, cancel))
        {
            return true;
        }

#pragma warning disable IDE0046 // Use conditional expression for return
        if (await IsPartitionLockedAsync(context.CompositeKey, sessionCfg.CacheAppName, cancel))
        {
            return true;
        }
#pragma warning restore IDE0046 // Use conditional expression for return
        return false;
    }

    public async Task RegisterFailureAsync(ThrottlingContext context, CancellationToken cancel = default)
    {
        var cfg = _settings.AuthSettings.SessionSettings;
        if (context.UserKey is not null)
        {
            await UpdatePartitionAsync(context.UserKey, TimeSpan.FromMinutes(cfg.UserWindowMinutes), cfg.UserLockThreshold, cfg, cancel);
        }
        await UpdatePartitionAsync(context.FingerprintKey, TimeSpan.FromMinutes(cfg.FingerprintWindowMinutes), cfg.FingerprintLockThreshold, cfg, cancel);
        await UpdatePartitionAsync(context.CompositeKey, TimeSpan.FromMinutes(cfg.CompositeWindowMinutes), cfg.CompositeLockThreshold, cfg, cancel);
    }

    public async Task ClearAsync(ThrottlingContext context, CancellationToken cancel = default)
    {
        var cfg = _settings.AuthSettings.SessionSettings;
        if (context.UserKey is not null)
        {
            await _cache.RemoveAsync(cfg.CacheAppName, context.UserKey, cancel);
        }
        await _cache.RemoveAsync(cfg.CacheAppName, context.FingerprintKey, cancel);
        await _cache.RemoveAsync(cfg.CacheAppName, context.CompositeKey, cancel);
    }

    private async Task<bool> IsPartitionLockedAsync(string key, string appName, CancellationToken cancel)
    {
        var state = await _cache.GetAsync<FailureState>(appName, key, cancel);
        return state?.LockUntilUtc is not null && state.LockUntilUtc > DateTime.UtcNow;
    }

    private async Task UpdatePartitionAsync(string key, TimeSpan window, int threshold, SessionSettings cfg, CancellationToken cancel)
    {
        var now = DateTime.UtcNow;
        var state = await _cache.GetAsync<FailureState>(cfg.CacheAppName, key, cancel) ?? new FailureState { FirstFailureUtc = now, Count = 0 };

        if (state.FirstFailureUtc + window < now)
        {
            state.FirstFailureUtc = now;
            state.Count = 0;
            state.LockUntilUtc = null;
        }

        state.Count++;
        if (state.Count >= threshold)
        {
            var over = state.Count - threshold;
            var lockSeconds = Math.Min(cfg.BaseLockSeconds * (int)Math.Pow(2, over), cfg.MaxLockSeconds);
            var until = now.AddSeconds(lockSeconds);
            if (state.LockUntilUtc is null || until > state.LockUntilUtc)
            {
                state.LockUntilUtc = until;
                _logger.LogWarning("Session auth throttling applied. Partition={Partition} Count={Count} LockSeconds={LockSeconds} Threshold={Threshold}", key, state.Count, lockSeconds, threshold);
            }
        }

        var absolute = state.LockUntilUtc.HasValue && state.LockUntilUtc > now ? state.LockUntilUtc.Value : state.FirstFailureUtc + window;
        var ttl = absolute - now;
        if (ttl < TimeSpan.FromSeconds(1))
        {
            ttl = TimeSpan.FromSeconds(1);
        }

        await _cache.SetAsync(cfg.CacheAppName, key, state, ttl, cancel);
    }

    [ProtoContract]
    private sealed class FailureState
    {
        [ProtoMember(1)] public int Count { get; set; }
        [ProtoMember(2)] public DateTime FirstFailureUtc { get; set; }
        [ProtoMember(3)] public DateTime? LockUntilUtc { get; set; }
    }
}
