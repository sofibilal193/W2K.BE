using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DFI.Common.Persistence.Interfaces;

public interface IDbContext : IAsyncDisposable
{
    DatabaseFacade Database { get; }

    bool HasActiveTransaction { get; }

    bool SupportsTransactions { get; }

    bool IsSql { get; }

    bool IsDisposed { get; }

    Task<int> SaveChangesAsync(CancellationToken cancel = default);

    Task<bool> SaveEntitiesAsync(CancellationToken cancel = default);

    Task<Guid?> BeginTransactionAsync(CancellationToken cancel = default);

    Task CommitTransactionAsync(CancellationToken cancel = default);

    void RollbackTransaction();

    IDbContextTransaction? GetCurrentTransaction();

    void Migrate();

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
