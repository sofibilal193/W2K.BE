using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Config.Persistence.Configurations;

public class OfficeConfigFieldConfiguration : BaseEntityConfiguration<Entities.OfficeConfigField>
{
    public override void Configure(EntityTypeBuilder<Entities.OfficeConfigField> builder)
    {
        base.Configure(builder);
        _ = builder.ToTableWithHistory(PersistenceConstants.OfficeConfigFieldsTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.OfficeType)
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.Category)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnOrder(3);
        _ = builder.Property(x => x.FieldType)
            .HasConversion<string>()
            .HasMaxLength(25)
            .IsRequired()
            .HasColumnOrder(4);
        _ = builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnOrder(5);
        _ = builder.Property(x => x.Description)
            .HasMaxLength(255)
            .HasColumnOrder(6);
        _ = builder.Property(x => x.DefaultValue)
            .HasColumnOrder(7);
        _ = builder.Property(x => x.DisplayOrder)
            .HasColumnOrder(8);
        _ = builder.Property(x => x.MinValue)
            .HasPrecision(18, 4)
            .HasColumnOrder(9);
        _ = builder.Property(x => x.MaxValue)
            .HasPrecision(18, 4)
            .HasColumnOrder(10);
        _ = builder.Property(x => x.RegexValidator)
            .HasMaxLength(255)
            .HasColumnOrder(11);
        _ = builder.Property(x => x.IsInternal)
            .HasColumnOrder(12);
        _ = builder.Property(x => x.IsEncrypted)
            .HasColumnOrder(13);

        // Has collections
        _ = builder.HasMany(x => x.Values)
            .WithOne(x => x.Field)
            .HasForeignKey(x => x.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Store FieldValues as JSON
        _ = builder.Property(x => x.FieldValues)
            .HasJsonConversion(PersistenceConstants.OfficeConfigFieldValuesJSONColumnName)
            .HasColumnOrder(14);

        // auto include navigation properties
        // Note: FieldValues is a JSON-serialized property, not a navigation, so AutoInclude doesn't apply to it
        _ = builder.Navigation(x => x.Values).AutoInclude(false);
    }
}
