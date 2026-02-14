namespace W2K.Common.Persistence.Settings;

public record PersistenceSettings
{
    public string DbConnectionString { get; init; } = string.Empty;

    public bool UseAzureAdAuth { get; init; } = true;

    public int MaxRetryCount { get; init; } = 3;

    public int MaxRetryDelaySeconds { get; init; } = 30;

    public bool EnableSensitiveDataLogging { get; init; }

    public int? HistoryRetentionDays { get; init; }
}
