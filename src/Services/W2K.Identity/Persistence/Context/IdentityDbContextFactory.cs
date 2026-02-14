using System.Diagnostics.CodeAnalysis;
using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Identity.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "Unit Tests not needed for DBContextFactory classes.")]
public class IdentityDbContextFactory : DesignTimeDbContextFactoryBase<IdentityDbContext>
{
    public override IdentityDbContext CreateNewInstance(
        DbContextOptions<IdentityDbContext> options,
        ILoggerFactory loggerFactory,
        IMediator mediator,
        IOptions<PersistenceSettings> settings)
    {
        const CryptoOptions? cryptoOptions = null;
        var logger = loggerFactory.CreateLogger<IdentityDbContext>();
        var cryptoLogger = loggerFactory.CreateLogger<CryptoProvider>();

        return new IdentityDbContext(
            options,
            logger,
            new CryptoProvider(cryptoOptions, cryptoLogger),
            new NoCurrentUser(),
            mediator,
            settings);
    }
}
