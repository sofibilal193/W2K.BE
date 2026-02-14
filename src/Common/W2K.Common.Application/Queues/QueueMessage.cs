namespace W2K.Common.Application.Queues;

public readonly record struct QueueMessage
{
    public int? OfficeId { get; init; }

    public int? UserId { get; init; }

    public int? LoanAppId { get; init; }

    public string Message { get; init; } = string.Empty;

    public DateTime CreateDateTimeUtc { get; init; } = DateTime.UtcNow;

    public string ErrorMessage { get; init; } = string.Empty;

    public short RetryCount { get; init; }

    public short MaxRetryCount { get; init; }

    public int RetryQueueVisibilityDelayMilliSeconds { get; init; }

    public QueueMessage()
    {
    }
}
