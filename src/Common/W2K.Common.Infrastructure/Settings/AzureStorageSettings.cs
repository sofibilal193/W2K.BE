using Azure.Core;

namespace W2K.Common.Infrastructure.Settings;

public record AzureStorageSettings
{
    public string PrimaryUri { get; init; } = "";

    public string GeoRedundantSecondaryUri { get; init; } = "";

    public string DefaultContainerName { get; init; } = "";

    public RetryOptions? RetryOptions { get; init; }

    public AzureStorageSettings() { }

    public AzureStorageSettings(AzureStorageSettings settings)
    {
        PrimaryUri = settings.PrimaryUri;
        GeoRedundantSecondaryUri = settings.GeoRedundantSecondaryUri;
        RetryOptions = settings.RetryOptions;
        DefaultContainerName = settings.DefaultContainerName;
    }
}
