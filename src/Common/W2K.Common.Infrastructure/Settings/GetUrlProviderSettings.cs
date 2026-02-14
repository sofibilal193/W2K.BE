using Microsoft.Extensions.Options;

namespace W2K.Common.Infrastructure.Settings;

public record GetUrlProviderSettings : UrlProviderSettings
{
    public GetUrlProviderSettings(IOptionsMonitor<InfraSettings> settingsAccessor)
        : base(settingsAccessor.CurrentValue.UrlProviderSettings)
    {
    }
}
