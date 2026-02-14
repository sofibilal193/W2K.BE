using System.Collections.Concurrent;
using W2K.Common.Events;
using W2K.Common.Persistence.Entities;
using W2K.Common.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace W2K.Common.Persistence.Events;

public class EventLogSink<TContext> : IEventLogSink
    where TContext : IDbContext
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly ConcurrentQueue<EventLogNotification> _events = new();

    public EventLogSink(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task QueueEventAsync(EventLogNotification eventNotification, CancellationToken cancel = default)
    {
        _events.Enqueue(eventNotification);
        return Task.CompletedTask;
    }

    public async Task FlushQueueAsync(CancellationToken cancel = default)
    {
        if (_serviceProvider is not null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            while (context is not null && !context.IsDisposed && !_events.IsEmpty)
            {
                if (_events.TryDequeue(out EventLogNotification? itemDoc))
                {
                    context.Entry(new EventLog(itemDoc)).State = EntityState.Added;
                }
            }
            if (context is not null)
            {
                await context.SaveChangesAsync(CancellationToken.None);
            }
        }
    }
}
