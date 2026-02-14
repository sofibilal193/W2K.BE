using DFI.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DFI.Common.Persistence.Configurations;

public class EventLogConfiguration : IEntityTypeConfiguration<EventLog>
{
    public void Configure(EntityTypeBuilder<EventLog> builder)
    {
        _ = builder.ToTable(PersistenceConstants.EventLogsTableName, PersistenceConstants.EventLogsTableSchemaName);
        _ = builder.HasKey(x => x.Id);
        _ = builder.Property(x => x.Id).HasColumnOrder(1);
        _ = builder.Property(x => x.UserId).HasColumnOrder(2);
        _ = builder.Property(x => x.OfficeId).HasColumnOrder(3);
        _ = builder.Property(x => x.RecordId).HasColumnOrder(4);
        _ = builder.Property(x => x.Type).HasMaxLength(100).HasColumnOrder(5);
        _ = builder.Property(x => x.Description).HasColumnOrder(6);
        _ = builder.Property(x => x.DateTimeUtc).IsRequired().HasColumnOrder(7);
        _ = builder.Property(x => x.Source).HasMaxLength(50).HasColumnOrder(8);
        _ = builder.Property(x => x.Timestamp).IsRowVersion().HasColumnOrder(99);
        _ = builder.HasIndex(x => x.UserId).HasDatabaseName("IX_EventLogs_UserId");
        _ = builder.HasIndex(x => x.OfficeId).HasDatabaseName("IX_EventLogs_OfficeId");
        _ = builder.HasIndex(x => x.RecordId).HasDatabaseName("IX_EventLogs_RecordId");
        _ = builder.HasIndex(x => x.Type).HasDatabaseName("IX_EventLogs_Type");
    }
}
