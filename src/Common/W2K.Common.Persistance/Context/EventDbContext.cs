using DFI.Common.Crypto;
using DFI.Common.Identity;
using DFI.Common.Persistence.Configurations;
using DFI.Common.Persistence.Entities;
using DFI.Common.Persistence.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Persistence.Context;

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
