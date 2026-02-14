using System.Diagnostics.CodeAnalysis;
using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Files.Persistence.Context;

[ExcludeFromCodeCoverage(Justification = "Unit Tests not needed for DBContextFactory classes.")]
public class FilesDbContextFactory : DesignTimeDbContextFactoryBase<FilesDbContext>
{
    public override FilesDbContext CreateNewInstance(
        DbContextOptions<FilesDbContext> options,
        ILoggerFactory loggerFactory,
        IMediator mediator,
        IOptions<PersistenceSettings> settings)
    {
        const CryptoOptions? cryptoOptions = null;
        var logger = loggerFactory.CreateLogger<FilesDbContext>();
        var cryptoLogger = loggerFactory.CreateLogger<CryptoProvider>();

        return new FilesDbContext(
            options,
            logger,
            new CryptoProvider(cryptoOptions, cryptoLogger),
            new NoCurrentUser(),
            mediator,
            settings);
    }
}
