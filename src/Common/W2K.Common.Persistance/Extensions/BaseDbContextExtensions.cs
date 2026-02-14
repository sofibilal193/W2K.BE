using DFI.Common.Crypto;
using DFI.Common.Identity;
using DFI.Common.Persistence.Context;
using DFI.Common.Persistence.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Persistence.Extensions;

public static class BaseDbContextExtensions
{

    public static TContext Create<TContext>(
         ICryptoProvider cryptoProvider,
         ILogger<TContext> logger,
         ILoggerFactory testLoggerFactory,
         ICurrentUser currentUser,
         IMediator mediator,
         string? dbId = null) where TContext : BaseDbContext<TContext>
    {
        var databaseId = dbId ?? Guid.NewGuid().ToString(); // Unique per test if not provided
        var options = new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseId)
            .UseLoggerFactory(testLoggerFactory)
            .EnableSensitiveDataLogging()
            .Options;

        var settings = Options.Create(new PersistenceSettings());
        var contextObj = Activator.CreateInstance(typeof(TContext), options, logger, cryptoProvider, currentUser, mediator, settings);
        if (contextObj is not TContext context)
        {
            throw new InvalidOperationException($"Could not create an instance of type {typeof(TContext).FullName}.");
        }

        _ = context.Database.EnsureCreated();
        return context;
    }

    public static void Destroy<TContext>(BaseDbContext<TContext> context)
        where TContext : DbContext
    {
        _ = context.Database.EnsureDeleted();
        context.Dispose();
    }
}
