namespace W2K.Common.Infrastructure.Settings;

public record RedisCacheSettings
{
    public string ConnectionString { get; init; } = string.Empty;

    public string InstanceName { get; init; } = string.Empty;

    public int ConnectTimeoutMS { get; init; } = 5000;

    public int SyncTimeoutMS { get; init; } = 5000;

    public int AsyncTimeoutMS { get; init; } = 5000;

    public RedisCacheSettings() { }

    public RedisCacheSettings(RedisCacheSettings settings)
    {
        ConnectionString = settings.ConnectionString;
        InstanceName = settings.InstanceName;
        ConnectTimeoutMS = settings.ConnectTimeoutMS;
        SyncTimeoutMS = settings.SyncTimeoutMS;
        AsyncTimeoutMS = settings.AsyncTimeoutMS;
    }
}
