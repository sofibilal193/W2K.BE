#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using W2K.Common.Persistence.Security;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace W2K.Common.Persistence.Utils;

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public class CustomMigrationsSqlGenerator(MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer) : SqlServerMigrationsSqlGenerator(dependencies, commandBatchPreparer)
{
    private readonly ISqlGenerationHelper _sqlHelper = dependencies.SqlGenerationHelper;

    protected override void Generate(CreateTableOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
    {
        base.Generate(operation, model, builder, terminate);
        GenerateHistoryRetention(builder, operation);
        foreach (var columnOperation in operation.Columns)
        {
            GenerateSensitivityClassification(builder, columnOperation);
            GenerateDynamicDataMask(builder, columnOperation);
        }
    }

    protected override void Generate(AlterTableOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        base.Generate(operation, model, builder);
        GenerateHistoryRetention(builder, operation);
    }

    protected override void Generate(AddColumnOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate)
    {
        base.Generate(operation, model, builder, terminate);
        GenerateSensitivityClassification(builder, operation);
        GenerateDynamicDataMask(builder, operation);
    }

    protected override void Generate(AlterColumnOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        base.Generate(operation, model, builder);
        GenerateSensitivityClassification(builder, operation);
        GenerateDynamicDataMask(builder, operation);
    }

    private void GenerateHistoryRetention(MigrationCommandListBuilder builder, TableOperation operation)
    {
        var annotation = operation.FindAnnotation(CustomAnnotations.HistoryRetention);
        var oldAnnotation = operation is AlterTableOperation alterOperation
            ? alterOperation.OldTable.FindAnnotation(CustomAnnotations.HistoryRetention)
            : null;
        if (annotation is null || annotation.Value == oldAnnotation?.Value)
        {
            return;
        }

        var period = annotation.Value is null
            ? "INFINITE"
            : $"{annotation.Value} DAYS";
        _ = builder.Append($"ALTER TABLE {_sqlHelper.DelimitIdentifier(operation.Name, operation.Schema)}");
        _ = builder.Append($" SET (SYSTEM_VERSIONING = ON (HISTORY_RETENTION_PERIOD = {period}))");
        _ = builder.AppendLine(_sqlHelper.StatementTerminator).EndCommand();
    }

    private void GenerateSensitivityClassification(MigrationCommandListBuilder builder, ColumnOperation operation)
    {
        var identifier = $"{_sqlHelper.DelimitIdentifier(operation.Table, operation.Schema)}"
            + $".{_sqlHelper.DelimitIdentifier(operation.Name)}";
        var annotation = operation.FindAnnotation(CustomAnnotations.SensitivityClassification);
        if (annotation?.Value is null)
        {
            if (operation is AlterColumnOperation alterOperation
                && alterOperation.OldColumn.FindAnnotation(CustomAnnotations.SensitivityClassification) is not null)
            {
                _ = builder.Append($"DROP SENSITIVITY CLASSIFICATION FROM {identifier}")
                    .AppendLine(_sqlHelper.StatementTerminator)
                    .EndCommand();
            }
            return;
        }
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        var classification = JsonSerializer.Deserialize<SensitivityClassification>((string)annotation.Value, options)!;

        _ = builder.Append($"ADD SENSITIVITY CLASSIFICATION TO {identifier} ")
            .Append($"WITH (LABEL = '{classification.Label}', ")
            .Append($"INFORMATION_TYPE = '{classification.InformationType}', ")
            .Append($"RANK = {classification.Rank})")
            .AppendLine(_sqlHelper.StatementTerminator)
            .EndCommand();
    }

    private void GenerateDynamicDataMask(MigrationCommandListBuilder builder, ColumnOperation operation)
    {
        var annotation = operation.FindAnnotation(CustomAnnotations.DynamicDataMask);
        var oldAnnotation = operation is AlterColumnOperation alterOperation
            ? alterOperation.OldColumn.FindAnnotation(CustomAnnotations.DynamicDataMask)
            : null;
        var hasAnnotation = annotation?.Value is not null;
        var hadAnnotation = oldAnnotation?.Value is not null;
        if (!hasAnnotation && !hadAnnotation)
        {
            return;
        }
        if (hasAnnotation && hadAnnotation && annotation?.Value == oldAnnotation?.Value)
        {
            return;
        }

        _ = builder.Append($"ALTER TABLE {_sqlHelper.DelimitIdentifier(operation.Table, operation.Schema)} ")
            .Append($"ALTER COLUMN {_sqlHelper.DelimitIdentifier(operation.Name)} ");
        if (hadAnnotation && !hasAnnotation)
        {
            _ = builder.Append("DROP MASKED");
        }
        else
        {
            if (!hadAnnotation && hasAnnotation)
            {
                _ = builder.Append("ADD ");
            }
            _ = builder.Append($"MASKED WITH (FUNCTION = '{annotation?.Value}')");
        }
        _ = builder.AppendLine(_sqlHelper.StatementTerminator).EndCommand();
    }
}
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
