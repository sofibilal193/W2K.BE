using DFI.Common.Entities;
using DFI.Common.Persistence.Extensions;
using DFI.Common.Persistence.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DFI.Common.Persistence.Configurations;

public abstract class BaseEntityConfiguration<TBase> : IEntityTypeConfiguration<TBase>
    where TBase : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TBase> builder)
    {
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnOrder(1);
        _ = builder.Property(x => x.IsDisabled)
            .HasColumnOrder(81);
        _ = builder.Property(x => x.CreateDateTimeUtc)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnOrder(91);
        _ = builder.Property(x => x.CreateUserId)
            .HasColumnOrder(92);
        _ = builder.Property(x => x.CreateUserName)
            .HasMaxLength(100)
            .HasColumnOrder(93);
        _ = builder.Property(x => x.CreateSource)
            .HasMaxLength(50)
            .HasColumnOrder(94)
            .HasSensitivityClassification(
                SensitivityLabels.ConfidentialGdpr,
                SensitivityInformationTypes.Networking,
                SensitivityRank.MEDIUM);
        _ = builder.Property(x => x.ModifyDateTimeUtc)
            .HasDefaultValueSql("GETUTCDATE()")
            .HasColumnOrder(95);
        _ = builder.Property(x => x.ModifyUserId)
            .HasColumnOrder(96);
        _ = builder.Property(x => x.ModifyUserName)
            .HasMaxLength(100)
            .HasColumnOrder(97);
        _ = builder.Property(x => x.ModifySource)
            .HasMaxLength(50)
            .HasColumnOrder(98)
            .HasSensitivityClassification(
                SensitivityLabels.ConfidentialGdpr,
                SensitivityInformationTypes.Networking,
                SensitivityRank.MEDIUM);
        _ = builder.Property(x => x.Timestamp)
            .IsRowVersion()
            .HasColumnOrder(99);
        _ = builder.Ignore(x => x.DomainEvents);
    }
}
