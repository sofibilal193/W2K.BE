namespace W2K.Common.Infrastructure.Settings;

public record InfraSettings
{
    #region Azure App Configuration Settings

    public string AzureAppConfigUri { get; init; } = string.Empty;

    // https://docs.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-aspnet-core?tabs=core5x
    public string AzureAppConfigSentinelKey { get; init; } = string.Empty;

    public TimeSpan AzureAppConfigCachePeriod { get; init; }

    #endregion

    #region Api Configurations

    public ApiServiceSettings ApiServiceSettings { get; init; } = new();

    #endregion

    #region Twilio Settings

    public TwilioSettings TwilioSettings { get; set; } = new();

    #endregion

    #region SendGrid Settings

    public string SendGridApiKey { get; set; } = string.Empty;

    #endregion

    #region Url Provider Settings

    public UrlProviderSettings UrlProviderSettings { get; set; } = new();

    #endregion

    #region Redis Cache Settings

    public RedisCacheSettings RedisCacheSettings { get; init; } = new();

    #endregion

    #region Azure Storage Settings

    public AzureStorageSettings AzureStorageSettings { get; init; } = new();

    #endregion

    #region AzureAD Settings

    public AzureADSettings AzureAD { get; init; } = new();

    #endregion

    #region Session Store Settings

    public SessionStoreSettings SessionStoreSettings { get; init; } = new();

    #endregion
}
