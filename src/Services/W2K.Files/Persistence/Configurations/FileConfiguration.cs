using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Security;
using W2K.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = W2K.Files.Entities.File;

namespace W2K.Files.Persistence.Configurations;

public class FileConfiguration : BaseEntityConfiguration<File>
{
    public override void Configure(EntityTypeBuilder<File> builder)
    {
        base.Configure(builder);

        _ = builder.ToTableWithHistory(PersistenceConstants.FilesTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.Label)
            .HasMaxLength(255)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.Path)
            .HasMaxLength(255)
            .IsRequired()
            .HasColumnOrder(3)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.Networking,
                SensitivityRank.HIGH);
        _ = builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnOrder(4);

        // Owned collections
        _ = builder.OwnsMany(x => x.Tags, x =>
            {
                _ = x.ToTableWithHistory(PersistenceConstants.FileTagsTableName, PersistenceConstants.DefaultTableSchema);
                _ = x.WithOwner().HasForeignKey(PersistenceConstants.FileIdColumn);
                _ = x.Property<int>(PersistenceConstants.DefaultIdColumn)
                    .ValueGeneratedOnAdd()
                    .HasColumnOrder(1);
                _ = x.HasKey(PersistenceConstants.DefaultIdColumn);
                _ = x.Property(PersistenceConstants.FileIdColumn)
                    .IsRequired()
                    .HasColumnOrder(2);
                _ = x.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(3);
                _ = x.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnOrder(3);
                _ = x.HasIndex(PersistenceConstants.FileIdColumn, nameof(Tag.Name))
                    .HasDatabaseName("IX_FileTags_FileId_Name")
                    .IsUnique();
            });

    }

}
