using System;
using System.Diagnostics.CodeAnalysis;
using DFI.Common.Application.Cacheing;
using DFI.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DFI.Common.Application.BackgroundServices;

[ExcludeFromCodeCoverage(Justification = "Abstract background service does not need unit tests.")]
[SuppressMessage("Maintainability", "T0038", Justification = "We need to suppress this rule here, because the abstract class has some false positives that do not apply.")]

public abstract class SingleBackgroundService : BackgroundService
{
    private const string AppName = "BackgroundService";

    private readonly ICache _cache;
    private readonly BackgroundServiceConfig _serviceConfig;
    private readonly string _hostName;
    private bool _isActiveInstance;
    private bool _isOneTimeExecutionCompleted;

    public abstract Task DoWorkAsync(CancellationToken cancel);

    protected ILogger Logger { get; init; }
    protected IServiceProvider? ServiceProvider { get; init; }
    protected string WorkerName { get; init; }


    protected SingleBackgroundService(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime lifetime,
        ILogger logger,
        BackgroundServiceConfig config)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _serviceConfig = config ?? throw new ArgumentNullException(nameof(config));

        using var scope = ServiceProvider.CreateScope();
        _cache = scope.ServiceProvider.GetRequiredService<ICache>();
        WorkerName = config.WorkerName ?? GetType().FullName ?? GetType().Name;
        _hostName = OSUtil.GetHostName();
        lifetime.ApplicationStopped.Register(async () => await StopAsync(CancellationToken.None));
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopServiceAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        if (_cache is IDisposable disposableCache)
        {
            disposableCache.Dispose();
        }
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        Logger.LogInformation(
            "{WorkerName} has {StartDelay} start delay configured.",
            WorkerName,
            _serviceConfig.StartDelay);
        Logger.LogInformation(
            "{WorkerName} has {IntervalPeriod} interval period configured.",
            WorkerName,
            _serviceConfig.IntervalPeriod);

        try
        {
            if (!_serviceConfig.IsEnabled)
            {
                Logger.LogInformation("{WorkerName} is disabled.", WorkerName);
                return;
            }

            var leaseCacheKey = $"{WorkerName}.Lease";
            var timer = new Timer(async (_) => await UpsertLeaseAsync(leaseCacheKey, stoppingToken));
            timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(_serviceConfig.HeartBeatMinutes));

            await Task.Delay(_serviceConfig.StartDelay, stoppingToken);

            while (!_isOneTimeExecutionCompleted && !stoppingToken.IsCancellationRequested)
            {
                await GetActiveLeaseAsync(leaseCacheKey, stoppingToken);
                if (_isActiveInstance && !IsInMaintenanceWindow())
                {
                    await TryDoWorkAsync(stoppingToken);
                }

                if (_serviceConfig.IntervalPeriod.HasValue)
                {
                    await Task.Delay(_serviceConfig.IntervalPeriod.Value, stoppingToken);
                }
            }

            Logger.LogInformation(
                "{WorkerName} execution ended on {HostName}. Cancellation token = {IsCancellationRequested}",
                WorkerName,
                _hostName,
                stoppingToken.IsCancellationRequested);
        }
        catch (Exception ex) when (stoppingToken.IsCancellationRequested)
        {
            Logger.LogWarning(ex, "{WorkerName} execution cancelled on {HostName}.", WorkerName, _hostName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception. {WorkerName} execution stopping on {HostName}.", WorkerName, _hostName);
        }
    }

    protected async Task StopServiceAsync(CancellationToken cancel)
    {
        Logger.LogInformation("{WorkerName} is stopping on {HostName}.", WorkerName, _hostName);
        if (_isActiveInstance)
        {
            var cacheKey = $"{WorkerName}.Lease";
            await _cache.RemoveAsync(AppName, cacheKey, cancel);
            await _cache.RemoveAsync(AppName, $"{WorkerName}.LeasePending", cancel);
            Logger.LogInformation("Removed lease: {Key} from cache.", cacheKey);
        }
    }

    private async Task UpsertLeaseAsync(string cacheKey, CancellationToken cancel)
    {
        var lease = await GetActiveLeaseAsync(cacheKey, cancel);
        if (lease is null)
        {
            _isActiveInstance = true;
            Logger.LogInformation(
                "{WorkerName} has been leased to {LeaseHost} (Heartbeat: {HeartBeat} min.).",
                WorkerName,
                _hostName,
                _serviceConfig.HeartBeatMinutes);
        }
        if (_isActiveInstance)
        {
            await _cache.SetAsync(AppName, cacheKey, $"{_hostName}|{DateTime.UtcNow.Ticks}", cancel);
        }
    }

    private async Task<string?> GetActiveLeaseAsync(string cacheKey, CancellationToken cancel)
    {
        try
        {
            _isActiveInstance = false;
            var lease = await _cache.GetAsync<string>(AppName, cacheKey, cancel);
            if (lease is null)
            {
                return null;
            }

            var parts = lease.Split('|');
            var leaseHost = parts[0];
            var lastActive = long.TryParse(parts[1], out var ticks) ? new DateTime(ticks, DateTimeKind.Utc) : DateTime.MinValue;
            if (leaseHost == _hostName)
            {
                _isActiveInstance = true;
            }
            else if (DateTime.UtcNow.Subtract(lastActive).TotalMinutes > (_serviceConfig.HeartBeatMinutes + 1))
            {
                lease = null;
                Logger.LogInformation(
                    "{WorkerName} on {LeaseHost} is stale. Last Active: {LastActive} UTC.",
                    WorkerName,
                    _hostName,
                    lastActive);
            }
            return lease;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting active lease for {WorkerName}.", WorkerName);
            return null;
        }
    }

    private async Task TryDoWorkAsync(CancellationToken cancel)
    {
        try
        {
            await DoWorkAsync(cancel);
            if (!_serviceConfig.IntervalPeriod.HasValue)
            {
                _isOneTimeExecutionCompleted = true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Unhandled exception occurred in the {Worker}. Sending an alert. Worker will retry after the normal interval.",
                WorkerName);
        }
    }

    private bool IsInMaintenanceWindow()
    {
        var currentTime = DateTime.UtcNow.TimeOfDay;
        return _serviceConfig.MaintenanceStartTimeUtc.HasValue && _serviceConfig.MaintenanceEndTimeUtc.HasValue
            && currentTime >= _serviceConfig.MaintenanceStartTimeUtc.Value && currentTime <= _serviceConfig.MaintenanceEndTimeUtc.Value;
    }
}
