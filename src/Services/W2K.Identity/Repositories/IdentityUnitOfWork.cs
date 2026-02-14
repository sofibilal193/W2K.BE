using W2K.Common.Persistence.Repositories;
using W2K.Identity.Persistence.Context;

namespace W2K.Identity.Repositories;

public class IdentityUnitOfWork(
    IdentityDbContext context,
    IOfficeRepository offices,
    IUserRepository users,
    IRoleRepository roles,
    IPermissionRepository permissions,
    IOfficeUserRepository officeUsers,
    ISessionLogsRepository sessionLogs) : IIdentityUnitOfWork
{
    private readonly IdentityDbContext _context = context;
    private bool _disposedValue;

    public IOfficeRepository Offices { get; } = offices;

    public IUserRepository Users { get; } = users;

    public IRoleRepository Roles { get; } = roles;

    public IPermissionRepository Permissions { get; } = permissions;

    public IOfficeUserRepository OfficeUsers { get; } = officeUsers;

    public ISessionLogsRepository SessionLogs { get; } = sessionLogs;

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
