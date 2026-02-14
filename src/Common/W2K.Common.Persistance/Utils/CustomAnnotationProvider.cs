using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.SqlServer.Metadata.Internal;

namespace DFI.Common.Persistence.Utils;
#pragma warning disable EF1001 // Internal EF Core API usage.

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public class CustomAnnotationProvider(RelationalAnnotationProviderDependencies dependencies) : SqlServerAnnotationProvider(dependencies)
{
    public override IEnumerable<IAnnotation> For(ITable table, bool designTime)
    {
        var annotations = base.For(table, designTime).ToList();
        var annotation = table.EntityTypeMappings.First().TypeBase.FindAnnotation(CustomAnnotations.HistoryRetention);
        if (annotation is not null && !annotations.Any(x => x.Name == annotation.Name))
        {
            annotations.Add(annotation);
        }
        return annotations;
    }

    public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
    {
        var annotations = base.For(column, designTime).ToList();
        if (column.PropertyMappings.Count > 0)
        {
            var customAnnotations = column.PropertyMappings[0]
                .Property
                .GetAnnotations()
                .Where(
                    x =>
                    x.Name == CustomAnnotations.DynamicDataMask || x.Name == CustomAnnotations.SensitivityClassification)
                .Where(x => !annotations.Any(a => a.Name == x.Name));

            annotations.AddRange(customAnnotations);
        }
        return annotations;
    }
}
#pragma warning restore EF1001 // Internal EF Core API usage.
