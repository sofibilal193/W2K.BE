namespace W2K.Common.Persistence.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancel = default);

    Task<bool> SaveEntitiesAsync(CancellationToken cancel = default);
}
