using Microsoft.Extensions.Options;

namespace DFI.Common.Infrastructure.Settings;

public record GetTwilioSettings : TwilioSettings
{
    public GetTwilioSettings(IOptionsMonitor<InfraSettings> settingsAccessor)
        : base(settingsAccessor.CurrentValue.TwilioSettings)
    {
    }
}
