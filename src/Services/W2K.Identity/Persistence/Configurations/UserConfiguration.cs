using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Security;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Identity.Persistence.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        _ = builder.ToTableWithHistory(PersistenceConstants.UsersTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.ProviderId)
            .HasMaxLength(50)
            .HasColumnOrder(2);
        _ = builder.HasIndex(x => new { x.ProviderId }).IsUnique();
        _ = builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnOrder(3)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.Name,
                SensitivityRank.MEDIUM);
        _ = builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnOrder(4)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.Name,
                SensitivityRank.MEDIUM);
        _ = builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100)
            .HasDataMask()
            .HasColumnOrder(5)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.ContactInfo,
                SensitivityRank.MEDIUM);
        _ = builder.HasIndex(x => x.Email).IsUnique();
        _ = builder.Property(x => x.MobilePhone)
            .HasMaxLength(50)
            .HasDataMask()
            .HasColumnOrder(6)
            .HasSensitivityClassification(
                SensitivityLabels.Confidential,
                SensitivityInformationTypes.ContactInfo,
                SensitivityRank.MEDIUM);
        _ = builder.Property(x => x.LastLoginDateTimeUtc)
            .HasColumnOrder(7);
        _ = builder.Property(x => x.LastLogoutDateTimeUtc)
            .HasColumnOrder(8);

        // Ignored Properties
        _ = builder.Ignore(x => x.DomainEvents);

        // Has collections
        _ = builder.HasMany(x => x.Offices)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId);
    }
}
