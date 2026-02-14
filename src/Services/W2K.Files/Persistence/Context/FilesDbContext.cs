using W2K.Common.Crypto;
using W2K.Common.Identity;
using W2K.Common.Persistence.Context;
using W2K.Common.Persistence.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using File = W2K.Files.Entities.File;

namespace W2K.Files.Persistence.Context;

public sealed class FilesDbContext(
    DbContextOptions<FilesDbContext> options,
    ILogger<FilesDbContext> logger,
    ICryptoProvider cryptoProvider,
    ICurrentUser currentUser,
    IMediator mediator,
    IOptions<PersistenceSettings> settings) : BaseDbContext<FilesDbContext>(options, logger, cryptoProvider, currentUser, mediator, settings)
{
    public override string DefaultSchema => PersistenceConstants.DefaultTableSchema;

    public DbSet<File> Files => Set<File>();
}
