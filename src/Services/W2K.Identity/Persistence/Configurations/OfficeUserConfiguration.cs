using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace W2K.Identity.Persistence.Configurations;

public class OfficeUserConfiguration : BaseEntityConfiguration<OfficeUser>
{
    public override void Configure(EntityTypeBuilder<OfficeUser> builder)
    {
        base.Configure(builder);
        _ = builder.ToTableWithHistory(PersistenceConstants.OfficeUsersTableName, PersistenceConstants.DefaultTableSchema);

        _ = builder.Property(x => x.OfficeId)
            .IsRequired()
            .HasColumnOrder(2);
        _ = builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnOrder(3);
        _ = builder.Property(x => x.RoleId)
            .HasColumnOrder(4);
        _ = builder.Property(x => x.Title)
            .HasMaxLength(50)
            .HasColumnOrder(5);
        _ = builder.Property(x => x.IsDefault)
            .IsRequired()
            .HasColumnOrder(6);
        _ = builder.Property(x => x.IsInvited)
            .IsRequired()
            .HasColumnOrder(7);
        _ = builder.Property(x => x.IsInviteProcessed)
            .IsRequired()
            .HasColumnOrder(8);
        _ = builder.Property(x => x.Department)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(25)
            .HasColumnOrder(9);
        _ = builder.Property(x => x.IsManager)
            .HasColumnOrder(10);
        _ = builder.Property(x => x.InviteAcceptedDateTimeUtc)
            .HasColumnOrder(11);

        // Has collections
        _ = builder.HasOne(x => x.Office)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.OfficeId)
            .OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasOne(x => x.User)
            .WithMany(x => x.Offices)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        _ = builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
