using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using DFI.Common.Persistence.Encryption;
using DFI.Common.Persistence.Security;
using DFI.Common.Persistence.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DFI.Common.Persistence.Extensions;

[ExcludeFromCodeCoverage(Justification = "Excluded from code coverage as we don't need coverage for this class.")]
public static class PropertyBuilderExtensions
{
    /// <summary>
    /// Indicates that this field should be encrypted in the data store. Encryption is applied via a value converter in
    /// DFI.Common.Persistence.Context.BaseDbContext.InitEncryptionValueConverter()
    /// </summary>
    /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
    /// <param name="algorithm">Algorithm to use for encryption. (Currently not implemented)</param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<string> HasEncryption(
         this PropertyBuilder<string> propertyBuilder,
         EncryptionAlgorithm algorithm = EncryptionAlgorithm.AES256)
    {
        return propertyBuilder.HasAnnotation(CustomAnnotations.Encryption, algorithm);
    }

    /// <summary>
    /// Adds metadata about the sensitivity classification to the associated database column.
    /// </summary>
    /// <typeparam name="T">Property type mask is being added to.</typeparam>
    /// <param name="propertyBuilder"></param>
    /// <param name="label">Human readable name of the sensitivity label.
    /// Sensitivity labels represent the sensitivity of the data stored in the database column.</param>
    /// <param name="informationType">Human readable name of the information type.
    /// Information types are used to describe the type of data being stored in the database column.</param>
    /// <param name="rank">Identifier based on a predefined set of values which define sensitivity rank.
    /// Used by other services like Advanced Threat Protection to detect anomalies based on their rank.</param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<T> HasSensitivityClassification<T>(
         this PropertyBuilder<T> propertyBuilder,
         string label,
         string informationType,
         SensitivityRank rank)
    {
        var classification = new SensitivityClassification(label, informationType, rank);
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        var options = new JsonSerializerOptions();
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        options.Converters.Add(new JsonStringEnumConverter());
        return propertyBuilder.HasAnnotation(CustomAnnotations.SensitivityClassification, JsonSerializer.Serialize(classification, options));
    }

    /// <summary>
    /// Indicates that this field should be masked when selected in a query.
    /// </summary>
    /// <typeparam name="T">Property type mask is being added to.</typeparam>
    /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
    /// <param name="function">Masking function to apply. Available functions: <see cref="DataMaskFunctions">DataMaskFunctions</see></param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<T> HasDataMask<T>(this PropertyBuilder<T> propertyBuilder, string? function = "default()")
    {
        return propertyBuilder.HasAnnotation(CustomAnnotations.DynamicDataMask, function);
    }

    /// <summary>
    /// Configures a property to be stored as JSON in the database using System.Text.Json serialization.
    /// </summary>
    /// <typeparam name="T">The CLR type of the property being configured.</typeparam>
    /// <param name="propertyBuilder">Property builder API for configuring a Microsoft.EntityFrameworkCore.Metadata.IMutableProperty.</param>
    /// <param name="columnName">Optional column name to use for the property. If null, the default column name is used.</param>
    /// <returns>The same builder instance so that multiple configuration calls can be chained.</returns>
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder, string? columnName = null)
    {
        var builder = propertyBuilder
            .HasConversion(
                x => JsonSerializer.Serialize(x, (JsonSerializerOptions?)null),
                x => JsonSerializer.Deserialize<T>(x, (JsonSerializerOptions?)null)!)
            .HasColumnType("nvarchar(max)");

        if (!string.IsNullOrEmpty(columnName))
        {
            _ = builder.HasColumnName(columnName);
        }

        return builder;
    }
}
