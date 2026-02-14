namespace W2K.Common.Application.Commands.Messaging;

/// <summary>
/// Response after creating or updating a message.
/// </summary>
/// <param name="ThreadId">The ID of the thread containing the message.</param>
/// <param name="MessageId">The unique identifier of the created or updated message.</param>
public readonly record struct UpsertMessageResponse(int ThreadId, Guid MessageId);
