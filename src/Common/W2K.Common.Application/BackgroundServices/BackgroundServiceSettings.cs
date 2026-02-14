namespace DFI.Common.Application.BackgroundServices;

/// <summary>
/// Root configuration settings for all background services.
/// </summary>
public sealed class BackgroundServiceSettings
{
    /// <summary>
    /// Dictionary of background service configurations keyed by service name.
    /// </summary>
    public Dictionary<string, BackgroundServiceConfig> Services { get; init; } = new();
}

/// <summary>
/// Configuration settings for background services.
/// </summary>
public sealed class BackgroundServiceConfig
{
    /// <summary>
    /// The name of the worker.
    /// </summary>
    public string? WorkerName { get; init; }

    /// <summary>
    /// whether the background service is enabled.
    /// </summary>
    public bool IsEnabled { get; init; } = true;

    /// <summary>
    /// The initial delay in seconds before the service starts executing.
    /// Used when DailyStartTimeUtc is not set.
    /// </summary>
    public int? StartDelaySeconds { get; init; }

    /// <summary>
    /// The interval in seconds between executions.
    /// </summary>
    public int? IntervalPeriodSeconds { get; init; }

    /// <summary>
    /// The interval in milliseconds between executions.
    /// Takes precedence over IntervalPeriodSeconds if both are set.
    /// </summary>
    public int? IntervalPeriodMilliseconds { get; init; }

    /// <summary>
    /// The UTC time of day when the service should start executing daily.
    /// When set, the service runs once per day at this time.
    /// </summary>
    public TimeSpan? DailyStartTimeUtc { get; init; }

    /// <summary>
    /// The heartbeat interval in minutes for lease management.
    /// </summary>
    public int HeartBeatMinutes { get; init; } = 15;

    /// <summary>
    /// The UTC time when maintenance window starts. Work will not execute during this window.
    /// </summary>
    public TimeSpan? MaintenanceStartTimeUtc { get; init; }

    /// <summary>
    /// The UTC time when maintenance window ends. Work will not execute during this window.
    /// </summary>
    public TimeSpan? MaintenanceEndTimeUtc { get; init; }

    /// <summary>
    /// The number of days before resetting the cache. Optional.
    /// </summary>
    public int? DaysToResetCache { get; init; }

    /// <summary>
    /// Gets the computed start delay for the service.
    /// If DailyStartTimeUtc is set, calculates delay until that time (or next day if already passed).
    /// Otherwise uses StartDelaySeconds (defaults to 5 seconds).
    /// </summary>
    public TimeSpan StartDelay
    {
        get
        {
            if (DailyStartTimeUtc.HasValue)
            {
                var start = DateTime.UtcNow.Date.Add(DailyStartTimeUtc.Value);
                // if start time has already elapsed, set to same start time tomorrow
                if (start < DateTime.UtcNow)
                {
                    start = DateTime.UtcNow.Date.AddDays(1).Add(DailyStartTimeUtc.Value);
                }
                return start.Subtract(DateTime.UtcNow);
            }
            return TimeSpan.FromSeconds(StartDelaySeconds ?? 5);
        }
    }

    /// <summary>
    /// Gets the computed interval period for the service.
    /// If DailyStartTimeUtc is set, returns the delay until next daily execution.
    /// Otherwise uses IntervalPeriodMilliseconds or IntervalPeriodSeconds (in that priority).
    /// Returns null if no interval is configured (service runs once).
    /// </summary>
    public TimeSpan? IntervalPeriod
    {
        get
        {
            // if a daily start time is set, interval period should run at same time every day
            if (DailyStartTimeUtc.HasValue)
            {
                return StartDelay;
            }
            else if (IntervalPeriodMilliseconds.HasValue)
            {
                return TimeSpan.FromMilliseconds(IntervalPeriodMilliseconds.Value);
            }
            else if (IntervalPeriodSeconds.HasValue)
            {
                return TimeSpan.FromSeconds(IntervalPeriodSeconds.Value);
            }

            return null;
        }
    }
}
