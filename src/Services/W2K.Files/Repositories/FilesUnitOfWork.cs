using W2K.Files.Persistence.Context;

namespace W2K.Files.Repositories;

public class FilesUnitOfWork(
    FilesDbContext context,
    IFilesRepository files) : IFilesUnitOfWork
{
    private readonly FilesDbContext _context = context;
    private bool _disposedValue;

    public IFilesRepository Files { get; } = files;

    public async Task<int> SaveChangesAsync(CancellationToken cancel = default)
    {
        return await _context.SaveChangesAsync(cancel);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancel = default)
    {
        return await _context.SaveEntitiesAsync(cancel);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposedValue = true;
        }
    }
}
