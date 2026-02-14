using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Config.Persistence.Context;

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

public sealed class ConfigDbContext(
    DbContextOptions<ConfigDbContext> options,
    ILogger<ConfigDbContext> logger,
    ICryptoProvider cryptoProvider,
    ICurrentUser currentUser,
    IMediator mediator,
    IOptions<PersistenceSettings> settings) : BaseDbContext<ConfigDbContext>(options, logger, cryptoProvider, currentUser, mediator, settings)
{
    public override string DefaultSchema => PersistenceConstants.DefaultTableSchema;

    public DbSet<Entities.Config> Configs => Set<Entities.Config>();

    public DbSet<Entities.OfficeConfigField> OfficeConfigFields => Set<Entities.OfficeConfigField>();

    public DbSet<Entities.OfficeConfigFieldValue> OfficeConfigFieldValues => Set<Entities.OfficeConfigFieldValue>();
}

#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
