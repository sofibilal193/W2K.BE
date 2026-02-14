using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Config.Persistence.Configurations;

public class ConfigConfiguration : BaseEntityConfiguration<Entities.Config>
{
    public override void Configure(EntityTypeBuilder<Entities.Config> builder)
    {
        base.Configure(builder);
        _ = builder.ToTableWithHistory(PersistenceConstants.ConfigsTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.Type)
            .HasMaxLength(50)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired()
            .HasColumnOrder(3);
        _ = builder.Property(x => x.Description)
            .HasMaxLength(255)
            .HasColumnOrder(4);
        _ = builder.Property(x => x.Value)
            .HasColumnOrder(5);
        _ = builder.Property(x => x.DisplayOrder)
            .HasColumnOrder(6);
        _ = builder.Property(x => x.IsInternal)
            .HasColumnOrder(7);
        _ = builder.Property(x => x.IsEncrypted)
            .HasColumnOrder(8);
    }
}
