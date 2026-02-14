using Microsoft.Extensions.Options;

namespace W2K.Common.Infrastructure.Settings;

public record GetAzureStorageSettings : AzureStorageSettings
{
    public GetAzureStorageSettings(IOptionsMonitor<InfraSettings> settingsAccessor)
        : base(settingsAccessor.CurrentValue.AzureStorageSettings)
    {
    }
}
