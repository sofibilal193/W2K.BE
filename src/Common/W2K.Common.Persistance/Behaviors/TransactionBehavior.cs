using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using W2K.Common.Persistence.Interfaces;
using W2K.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace W2K.Common.Persistence.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IDbContext? _dbContext;

    public TransactionBehavior(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<TransactionBehavior<TRequest, TResponse>>>();
        var dbContexts = serviceProvider.GetServices<IDbContext>();
        _dbContext = dbContexts.FirstOrDefault(x => x.SupportsTransactions)
            ?? dbContexts.FirstOrDefault();
    }

    public async Task<TResponse> Handle(
         TRequest request,
         RequestHandlerDelegate<TResponse> next,
         CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetGenericTypeName();

#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both
        try
        {
            if (_dbContext is null)
            {
                return await next(cancellationToken);
            }

            if (!_dbContext.SupportsTransactions)
            {
                return await next(cancellationToken);
            }

            if (_dbContext.HasActiveTransaction)
            {
                return await next(cancellationToken);
            }

            var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
                {
                    Guid? transactionId = await _dbContext.BeginTransactionAsync(cancellationToken);
                    _logger.LogDebug("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transactionId, typeName, request);
                    response = await next();
                    _logger.LogDebug("----- Commit transaction {TransactionId} for {CommandName}", transactionId, typeName);

                    if (transactionId.HasValue)
                    {
                        await _dbContext.CommitTransactionAsync(cancellationToken);
                    }
                });

            return response!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);
            throw;
        }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both

    }
}
