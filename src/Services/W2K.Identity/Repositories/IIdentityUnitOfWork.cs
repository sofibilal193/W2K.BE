using W2K.Common.Persistence.Repositories;

namespace W2K.Identity.Repositories;

public interface IIdentityUnitOfWork : IUnitOfWork
{
    IOfficeRepository Offices { get; }

    IUserRepository Users { get; }

    IRoleRepository Roles { get; }

    IPermissionRepository Permissions { get; }

    IOfficeUserRepository OfficeUsers { get; }

    ISessionLogsRepository SessionLogs { get; }
}
