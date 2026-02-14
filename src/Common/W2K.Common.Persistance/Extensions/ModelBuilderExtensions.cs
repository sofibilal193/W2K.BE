#pragma warning disable CA1863 // Use 'CompositeFormat'
using DFI.Common.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DFI.Common.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    private const string _historyTableFormat = "History_{0}";

    /// <summary>
    /// Configures the table that the entity type maps to when targeting a relational database.
    /// Also configures the table as a temporal table.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured.</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="useDefaultHistoryRetention">Whether to use the default history retention defined in the app configuration.</param>
    /// <param name="historyRetentionPeriod">Retention period to keep history records when useDefaultHistoryRetention is false (null = Infinate).</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> ToTableWithHistory<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        bool useDefaultHistoryRetention = true,
        TimeSpan? historyRetentionPeriod = null) where TEntity : class
    {
        var historyTable = string.Format(_historyTableFormat, entityTypeBuilder.Metadata.GetTableName());
        _ = entityTypeBuilder.ToTable(x => x.IsTemporal(t => t.UseHistoryTable(historyTable)));
        return entityTypeBuilder.HasAnnotation(CustomAnnotations.HistoryRetention, useDefaultHistoryRetention ? 0 : historyRetentionPeriod?.Days);
    }

    /// <summary>
    /// Configures the table that the entity type maps to when targeting a relational database.
    /// Also configures the table as a temporal table.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being configured.</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the table.</param>
    /// <param name="schema">The schema of the table.<param>
    /// <param name="useDefaultHistoryRetention">Whether to use the default history retention defined in the app configuration.</param>
    /// <param name="historyRetentionPeriod">Retention period to keep history records when useDefaultHistoryRetention is false (null = Infinate).</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static EntityTypeBuilder<TEntity> ToTableWithHistory<TEntity>(
        this EntityTypeBuilder<TEntity> entityTypeBuilder,
        string name,
        string? schema,
        bool useDefaultHistoryRetention = true,
        TimeSpan? historyRetentionPeriod = null) where TEntity : class
    {
        var historyTable = string.Format(_historyTableFormat, name);
        _ = entityTypeBuilder.ToTable(name, schema, x => x.IsTemporal(t => t.UseHistoryTable(historyTable, schema)));
        return entityTypeBuilder.HasAnnotation(CustomAnnotations.HistoryRetention, useDefaultHistoryRetention ? 0 : historyRetentionPeriod?.Days);
    }

    /// <summary>
    /// Configures the table that the entity type maps to when targeting a relational database.
    /// Also configures the table as a temporal table.
    /// </summary>
    /// <typeparam name="TOwnerEntity">The entity type owning the relationship.</typeparam>
    /// <typeparam name="TRelatedEntity">The dependent entity type of the relationship.</typeparam>
    /// <param name="entityTypeBuilder">The builder for the entity type being configured.</param>
    /// <param name="name">The name of the table.</param>
    /// <param name="schema">The schema of the table.<param>
    /// <param name="useDefaultHistoryRetention">Whether to use the default history retention defined in the app configuration.</param>
    /// <param name="historyRetentionPeriod">Retention period to keep history records when useDefaultHistoryRetention is false (null = Infinate).</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static OwnedNavigationBuilder<TOwnerEntity, TRelatedEntity> ToTableWithHistory<TOwnerEntity, TRelatedEntity>(
        this OwnedNavigationBuilder<TOwnerEntity, TRelatedEntity> referenceOwnershipBuilder,
        string name,
        string? schema,
        bool useDefaultHistoryRetention = true,
        TimeSpan? historyRetentionPeriod = null)
            where TOwnerEntity : class
            where TRelatedEntity : class
    {
        var historyTable = string.Format(_historyTableFormat, name);
        var builder = referenceOwnershipBuilder.ToTable(
            name,
            schema,
            x => x.IsTemporal(t => t.UseHistoryTable(historyTable)));
        return builder.HasAnnotation(CustomAnnotations.HistoryRetention, useDefaultHistoryRetention ? 0 : historyRetentionPeriod?.Days);
    }
}
#pragma warning restore CA1863 // Use 'CompositeFormat'
