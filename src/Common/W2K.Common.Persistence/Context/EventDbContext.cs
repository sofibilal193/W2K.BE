using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Entities;
using W2K.Common.Persistence.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Common.Persistence.Context;

public sealed class EventDbContext(
     DbContextOptions<EventDbContext> options,
     ILogger<EventDbContext> logger,
     ICryptoProvider cryptoProvider,
     ICurrentUser currentUser,
     IMediator mediator,
     IOptions<PersistenceSettings> settings) : BaseDbContext<EventDbContext>(options, logger, cryptoProvider, currentUser, mediator, settings)
{
    public override string DefaultSchema => "events";

    public DbSet<EventLog> EventLogs => Set<EventLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfiguration(new EventLogConfiguration());
    }
}
