namespace DFI.Common.Infrastructure.Settings;

public record TwilioSettings
{
    public string AccountSid { get; init; } = string.Empty;

    public string AuthToken { get; init; } = string.Empty;

    public TwilioSettings() { }

    public TwilioSettings(TwilioSettings settings)
    {
        AccountSid = settings.AccountSid;
        AuthToken = settings.AuthToken;
    }
}
