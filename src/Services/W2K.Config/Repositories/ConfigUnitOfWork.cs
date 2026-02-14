
using W2K.Config.Persistence.Context;

namespace W2K.Config.Repositories;

public class ConfigUnitOfWork(
    ConfigDbContext context,
    IConfigRepository configs,
    IOfficeConfigFieldRepository officeConfigFields,
    IOfficeConfigFieldValueRepository officeConfigFieldValues) : IConfigUnitOfWork
{
    private readonly ConfigDbContext _context = context;
    private bool _disposedValue;

    public IConfigRepository Configs { get; } = configs;

    public IOfficeConfigFieldRepository OfficeConfigFields { get; } = officeConfigFields;

    public IOfficeConfigFieldValueRepository OfficeConfigFieldValues { get; } = officeConfigFieldValues;

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
