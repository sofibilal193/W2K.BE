using System.Collections.ObjectModel;

namespace W2K.Common.Application.Messaging;

public readonly record struct EmailMessage
(
    EmailAddress From,
    ReadOnlyCollection<EmailAddress> To,
    string Subject,
    string Body,
    bool IsHtml = false,
    EmailAddress? ReplyTo = null,
    ReadOnlyCollection<EmailAddress>? Cc = null,
    ReadOnlyCollection<EmailAddress>? Bcc = null,
    string? TemplateId = null,
    object? TemplateData = null,
    int? UnSubscribeGroupId = null
);
