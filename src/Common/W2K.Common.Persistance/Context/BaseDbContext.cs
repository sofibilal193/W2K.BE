#pragma warning disable CA2215 // Dispose methods should call base class dispose
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DFI.Common.Crypto;
using DFI.Common.Entities;
using DFI.Common.Identity;
using DFI.Common.Persistence.Encryption;
using DFI.Common.Persistence.Extensions;
using DFI.Common.Persistence.Interfaces;
using DFI.Common.Persistence.Settings;
using DFI.Common.Persistence.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DFI.Common.Persistence.Context;

/// <summary>
/// BaseDbContext is a base class for all DbContext classes in the application.
/// It provides common functionality such as transaction management, encryption, and logging.
/// </summary>
/// <typeparam name="TContext">The type of the DbContext.</typeparam>
[ExcludeFromCodeCoverage(Justification = "Abstract class does not need unit tests.")]
public abstract class BaseDbContext<TContext>(
    DbContextOptions options,
    ILogger<BaseDbContext<TContext>> logger,
    ICryptoProvider cryptoProvider,
    ICurrentUser currentUser,
    IMediator mediator,
    IOptions<PersistenceSettings> settings) : DbContext(options), IDbContext
    where TContext : DbContext
{
        private IDbContextTransaction? _currentTransaction;
        private readonly PersistenceSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        private readonly ILogger<BaseDbContext<TContext>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    #pragma warning disable CA2213 // Disposable fields should be disposed - ICryptoProvider lifetime is managed by DI container
        private readonly ICryptoProvider _cryptoProvider = cryptoProvider ?? throw new ArgumentNullException(nameof(cryptoProvider));
    #pragma warning restore CA2213 // Disposable fields should be disposed
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    #region Public Properties

    public virtual string DefaultSchema => "base";

    public bool HasActiveTransaction => _currentTransaction is not null;

    public bool IsDisposed { get; private set; }

    public bool IsSql => Database.IsSqlServer();

    public bool SupportsTransactions => Database.IsRelational();

    #endregion

    #region Protected Properties

    protected ICurrentUser CurrentUser => _currentUser;
    protected ILogger<BaseDbContext<TContext>> Logger => _logger;
    protected ICryptoProvider CryptoProvider => _cryptoProvider;
    protected IMediator Mediator => _mediator;

    #endregion

    #region Public IDisposable Methods

    public sealed override void Dispose()
    {
        DoDispose(disposing: true);
#pragma warning disable S3971 // "GC.SuppressFinalize" should not be called
        GC.SuppressFinalize(this);
#pragma warning restore S3971 // "GC.SuppressFinalize" should not be called
    }

    // https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync
    public override async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        DoDispose(disposing: false);
#pragma warning disable S3971 // "GC.SuppressFinalize" should not be called
        GC.SuppressFinalize(this);
#pragma warning restore S3971 // "GC.SuppressFinalize" should not be called
    }

    #endregion

    #region Public Methods

    public IDbContextTransaction? GetCurrentTransaction()
    {
        return _currentTransaction;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        this.TrackEntityChanges(CurrentUser);
        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancel = default)
    {
        // Dispatch Domain Events collection.
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
        // performed through the DbContext will be committed
        _ = await SaveChangesAsync(cancel);

        // Decided to do this after Save() so that Views will get proper IDs on EntityCreated events.
        await Mediator.DispatchDomainEventsAsync(this, CurrentUser, cancel);
        return true;
    }

    public async Task<Guid?> BeginTransactionAsync(CancellationToken cancel = default)
    {
        if (!SupportsTransactions)
        {
            return null;
        }

        if (_currentTransaction is not null)
        {
            return _currentTransaction.TransactionId;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken: cancel);
        return _currentTransaction.TransactionId;
    }

    public async Task CommitTransactionAsync(CancellationToken cancel = default)
    {
        if (!HasActiveTransaction)
        {
            return;
        }
        try
        {
            _ = await SaveChangesAsync(cancel);
            if (_currentTransaction is not null)
            {
                await _currentTransaction.CommitAsync(cancel);
            }
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Migrate()
    {
        var applied = this.GetService<IHistoryRepository>()
            .GetAppliedMigrations()
            .Select(x => x.MigrationId);

        var total = this.GetService<IMigrationsAssembly>()
            .Migrations
            .Select(x => x.Key);

        if (total.Except(applied).Any())
        {
            Database.Migrate();
        }
    }

    #endregion

    #region Protected IDisposable Methods

    protected virtual void DoDispose(bool disposing)
    {
        if (disposing)
        {
            base.Dispose();
        }
        IsDisposed = true;
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        // dispose resources
        await base.DisposeAsync();
    }

    #endregion

    #region Protected Methods

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        _ = optionsBuilder.UseLazyLoadingProxies();
        _ = optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomMigrationsSqlGenerator>();
        _ = optionsBuilder.ReplaceService<IRelationalAnnotationProvider, CustomAnnotationProvider>();

        if (System.Diagnostics.Debugger.IsAttached)
        {
            _ = optionsBuilder.EnableDetailedErrors();
            _ = optionsBuilder.EnableSensitiveDataLogging();
        }

        // suppress exceptions for pending model changes
        // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0/breaking-changes#pending-model-changes
        _ = optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    protected void InitEncryptionValueConverter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                var annotation = property.FindAnnotation(CustomAnnotations.Encryption);
                if (annotation is not null)
                {
                    var converter = new EncryptedStringValueConverter(CryptoProvider);
                    property.SetValueConverter(converter);

                    // adjust max length to accomodate for length of encrypted strings
                    var maxLength = property.GetMaxLength();
                    if (maxLength.HasValue)
                    {
                        var testValue = new string('A', maxLength.Value);
                        var convertedValue = converter.ConvertToProvider.Invoke(testValue) as string;
                        if (convertedValue is not null)
                        {
                            property.SetMaxLength(convertedValue.Length);
                        }
                    }
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        Logger.LogInformation("BaseDbContext:Assembly: {Assembly}", GetType().Assembly);
        Logger.LogInformation("BaseDbContext:GetExecutingAssembly: {Assembly}", Assembly.GetExecutingAssembly());

        // apply configurations for all types that inherit from BaseEntity
        _ = modelBuilder.ApplyConfigurationsFromAssembly(
            GetType().Assembly,
            x => x.GetInterfaces().ToList().Exists(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)
                    && typeof(BaseEntity).IsAssignableFrom(i.GenericTypeArguments[0])));

        SetDefaultHistoryRetention(modelBuilder);
        InitEncryptionValueConverter(modelBuilder);
        _ = modelBuilder.HasDefaultSchema(schema: DefaultSchema);
    }

    #endregion

    #region Private Methods

    private void SetDefaultHistoryRetention(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var annotation = entityType.FindAnnotation(CustomAnnotations.HistoryRetention);
            if ((int?)annotation?.Value == 0)
            {
                entityType.SetAnnotation(CustomAnnotations.HistoryRetention, _settings.HistoryRetentionDays);
            }
        }
    }

    #endregion
}
#pragma warning restore CA2215 // Dispose methods should call base class dispose
