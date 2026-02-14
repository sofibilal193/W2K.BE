using DFI.Common.Entities;
using DFI.Common.Identity;
using DFI.Common.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DFI.Common.Persistence.Extensions;

public static class DbContextExtensions
{
    internal static void TrackEntityChanges(this DbContext ctx, ICurrentUser currentUser)
    {
        foreach (var entry in ctx.ChangeTracker.Entries().Where(x => x.Entity is BaseEntity or BaseEntityValueObject))
        {
            if (entry.State is EntityState.Detached or EntityState.Unchanged)
            {
                continue;
            }

            if (entry.Entity is BaseEntity baseEntity && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                baseEntity.Set(currentUser.UserId, currentUser.FullName, currentUser.Source);
            }
            else if (entry.Entity is BaseEntityValueObject baseEntityValueObject && (entry.State == EntityState.Added || entry.State == EntityState.Modified))
            {
                baseEntityValueObject.Set(currentUser.UserId, currentUser.FullName, currentUser.Source);
            }
        }
    }
}
