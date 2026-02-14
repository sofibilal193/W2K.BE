#pragma warning disable CA2000 // Dispose objects before losing scope
using System.Diagnostics.CodeAnalysis;
using DFI.Common.Crypto;
using DFI.Common.Identity;
using DFI.Common.Persistence.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "Tests not needed.")]
public class EventDbContextFactory : DesignTimeDbContextFactoryBase<EventDbContext>
{
    public override EventDbContext CreateNewInstance(
         DbContextOptions<EventDbContext> options,
         ILoggerFactory loggerFactory,
         IMediator mediator,
         IOptions<PersistenceSettings> settings)
    {
        const CryptoOptions? cryptoOptions = null;
        var logger = loggerFactory.CreateLogger<EventDbContext>();
        var cryptoLogger = loggerFactory.CreateLogger<CryptoProvider>();
        return new EventDbContext(options, logger, new CryptoProvider(cryptoOptions, cryptoLogger), new NoCurrentUser(), mediator, settings);
    }
}
#pragma warning restore CA2000 // Dispose objects before losing scope
