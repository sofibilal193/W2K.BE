using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace DFI.Common.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "This method handles infrastructure-specific logic and does not require unit test coverage.")]
public class NoMediator : IMediator
{
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
#pragma warning disable CS8604 // Possible null reference argument for parameter
        return Task.FromResult<TResponse>(default);
#pragma warning restore CS8604 // Possible null reference argument for parameter
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object?>(null);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        throw new NotSupportedException();
    }
}
