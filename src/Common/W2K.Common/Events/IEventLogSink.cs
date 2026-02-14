namespace W2K.Common.Events;

public interface IEventLogSink
{
    Task QueueEventAsync(
        EventLogNotification eventNotification,
        CancellationToken cancel = default);

    Task FlushQueueAsync(CancellationToken cancel = default);
}
