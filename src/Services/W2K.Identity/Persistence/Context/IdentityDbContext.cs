using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Settings;
using W2K.Identity.Entities;
using W2K.Identity.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Identity.Persistence.Context;

public sealed class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    ILogger<IdentityDbContext> logger,
    ICryptoProvider cryptoProvider,
    ICurrentUser currentUser,
    IMediator mediator,
    IOptions<PersistenceSettings> settings) : BaseDbContext<IdentityDbContext>(options, logger, cryptoProvider, currentUser, mediator, settings)
{
    public override string DefaultSchema => PersistenceConstants.DefaultTableSchema;

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<Office> Offices => Set<Office>();

    public DbSet<OfficeUser> OfficeUsers => Set<OfficeUser>();

    public DbSet<SessionLog> SessionLogs => Set<SessionLog>();
}
