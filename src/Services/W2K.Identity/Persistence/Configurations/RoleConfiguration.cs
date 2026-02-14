using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Identity.Persistence.Configurations;

public class RoleConfiguration : BaseEntityConfiguration<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);

        _ = builder.ToTableWithHistory(PersistenceConstants.RolesTableName, PersistenceConstants.DefaultTableSchema);
        _ = builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(25)
            .HasColumnOrder(2);
        _ = builder.Property(x => x.OfficeType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(25)
            .HasColumnOrder(3);
        _ = builder.Property(x => x.OfficeId)
            .HasColumnOrder(4);
        _ = builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnOrder(5);
        _ = builder.Property(x => x.Department)
            .HasMaxLength(100)
            .HasColumnOrder(6);
        _ = builder.Property(x => x.Description)
            .HasColumnOrder(7);

        // Ignored Properties
        _ = builder.Ignore(x => x.DomainEvents);

        // Has collections
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        _ = builder.HasMany(x => x.Permissions)
            .WithMany(x => x.Roles)
            .UsingEntity<Dictionary<string, object>>(
                PersistenceConstants.RolesPermissionsEntityName,
                x => x
                        .HasOne<Permission>()
                        .WithMany()
                        .HasForeignKey(PersistenceConstants.PermissionId)
                        .HasPrincipalKey(nameof(Permission.Id))
                        .OnDelete(DeleteBehavior.Cascade),
                x => x
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey(PersistenceConstants.RoleId)
                        .HasPrincipalKey(nameof(Role.Id))
                        .OnDelete(DeleteBehavior.Cascade),
                x =>
                {
                    _ = x.ToTableWithHistory(PersistenceConstants.RolesPermissionsTableName, PersistenceConstants.DefaultTableSchema);
                    _ = x.HasKey(PersistenceConstants.RoleId, PersistenceConstants.PermissionId);
                }
            );
    }
}
