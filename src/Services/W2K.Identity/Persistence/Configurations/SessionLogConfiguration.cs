using W2K.Common.Persistence.Configurations;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Security;
using W2K.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace W2K.Identity.Persistence.Configurations;

public class SessionLogConfiguration : BaseEntityConfiguration<SessionLog>
{
    public override void Configure(EntityTypeBuilder<SessionLog> builder)
    {
        base.Configure(builder);

        _ = builder.ToTable(PersistenceConstants.SessionLogsTableName, PersistenceConstants.DefaultTableSchema);

        _ = builder.Property(x => x.UserId)
            .HasColumnOrder(2)
            .IsRequired(false);

        _ = builder.Property(x => x.SessionId!)
            .HasMaxLength(255)
            .HasEncryption()
            .HasColumnOrder(3)
            .IsRequired(false);

        _ = builder.Property(x => x.Fingerprint!)
            .HasMaxLength(4000)
            .HasEncryption()
            .HasColumnOrder(4)
            .IsRequired(false);

        _ = builder.Property(x => x.EventType)
            .IsRequired()
            .HasColumnOrder(5)
            .HasConversion<string>();

        _ = builder.Property(x => x.OldSessionId!)
            .HasMaxLength(255)
            .HasEncryption()
            .HasColumnOrder(6)
            .IsRequired(false);
    }
}
