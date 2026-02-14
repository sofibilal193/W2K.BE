using W2K.Common.Persistence.Repositories;
using W2K.Identity.Entities;
using W2K.Identity.Persistence.Context;

namespace W2K.Identity.Repositories;

public class PermissionRepository(IdentityDbContext context) : DbRepository<Permission>(context), IPermissionRepository
{
}
