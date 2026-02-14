using W2K.Common.Application.Messaging;

namespace W2K.Identity.Settings;

public record ServiceSettings
{
    public Dictionary<string, EmailTemplate> EmailTemplates { get; init; } = [];

    public Dictionary<string, SmsTemplate> SmsTemplates { get; init; } = [];

    public string UserInvitationBaseUrl { get; init; } = "";

    public string ClientPortalBaseUrl { get; init; } = "";
}
