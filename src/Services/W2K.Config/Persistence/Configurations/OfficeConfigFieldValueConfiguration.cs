using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Config.Persistence.Configurations;

public class OfficeConfigFieldValueConfiguration : BaseEntityConfiguration<Entities.OfficeConfigFieldValue>
{
    public override void Configure(EntityTypeBuilder<Entities.OfficeConfigFieldValue> builder)
    {
        base.Configure(builder);
        _ = builder.ToTableWithHistory(PersistenceConstants.OfficeConfigFieldValuesTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.FieldId)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.OfficeId)
            .IsRequired()
            .HasColumnOrder(3);
        _ = builder.Property(x => x.Value)
            .HasColumnOrder(4);

        _ = builder.HasIndex(x => new { x.FieldId, x.OfficeId })
            .IsUnique()
            .HasDatabaseName("IX_OfficeConfigFieldValues_FieldId_OfficeId");

        // Has collections
        _ = builder.HasOne(x => x.Field)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
