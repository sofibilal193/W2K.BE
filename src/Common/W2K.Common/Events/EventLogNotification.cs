using MediatR;

namespace W2K.Common.Events;

public abstract record EventLogNotification(
    string EventType,
    DateTime DateTimeUtc,
    string? Source,
    string? Description,
    int? UserId,
    int? OfficeId,
    int? RecordId)
    : INotification;
