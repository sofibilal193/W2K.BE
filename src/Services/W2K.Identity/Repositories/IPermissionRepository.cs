using W2K.Common.Persistence.Repositories;
using W2K.Identity.Entities;

namespace W2K.Identity.Repositories;

public interface IPermissionRepository : IDbRepository<Permission>
{
}
