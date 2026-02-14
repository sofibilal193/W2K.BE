using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Identity.Persistence.Configurations;

public class PermissionConfiguration : BaseEntityConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        _ = builder.ToTableWithHistory(PersistenceConstants.PermissionsTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnOrder(2);
        _ = builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(25)
            .HasColumnOrder(3);
        _ = builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnOrder(4);
        _ = builder.HasIndex(x => x.Name).IsUnique();
        _ = builder.Property(x => x.Description)
            .HasColumnOrder(5);

        // Has collections
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        _ = builder.HasMany(x => x.Roles)
            .WithMany(x => x.Permissions)
            .UsingEntity<Dictionary<string, object>>(
                PersistenceConstants.RolesPermissionsEntityName,
                x => x
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey(PersistenceConstants.RoleId)
                        .HasPrincipalKey(nameof(Role.Id))
                        .OnDelete(DeleteBehavior.Cascade),
                x => x
                        .HasOne<Permission>()
                        .WithMany()
                        .HasForeignKey(PersistenceConstants.PermissionId)
                        .HasPrincipalKey(nameof(Permission.Id))
                        .OnDelete(DeleteBehavior.Cascade),
                x =>
                {
                    _ = x.ToTable(PersistenceConstants.RolesPermissionsTableName, PersistenceConstants.DefaultTableSchema);
                    _ = x.HasKey(PersistenceConstants.RoleId, PersistenceConstants.PermissionId);
                }
            );
    }
}
