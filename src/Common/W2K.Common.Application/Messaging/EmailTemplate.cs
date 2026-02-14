namespace W2K.Common.Application.Messaging;

public readonly record struct EmailTemplate
(
    string TemplateId,
    string FromName,
    string FromAddress,
    int? UnSubscribeGroupId,
    string? ToName,
    string? ToAddress
);
