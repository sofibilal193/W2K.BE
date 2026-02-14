using W2K.Common.Entities;
using W2K.Common.Identity;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Application.Enums;
using W2K.Identity.Entities;
using W2K.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace W2K.Identity.Repositories;

public class OfficeUserRepository(IdentityDbContext context) : DbRepository<OfficeUser>(context), IOfficeUserRepository
{

    private readonly IdentityDbContext _context = context;

    public async Task<PagedList<OfficeUser>> GetOfficeUsersAsync(int page, int pageSize, int userId, int? officeId, OfficeUsersSortColumn sortBy, bool sortDescending, CancellationToken cancel = default)
    {
        IEnumerable<int> targetOfficeIds;

        if (officeId.HasValue)
        {
            // If user specified a specific office, return users from that office
            targetOfficeIds = [officeId.Value];
        }
        else
        {
            // If no specific office requested, return users from all offices the current user belongs to
            targetOfficeIds = await GetUserOfficeIdsAsync(userId, cancel);
        }

        // Fetch all users in the target offices, excluding SuperAdmin users
        var query = _context.OfficeUsers
            .Include(x => x.User)
            .Include(x => x.Office)
            .AsNoTracking()
            .Where(x => targetOfficeIds.Contains(x.OfficeId) && x.Office!.Type != OfficeType.SuperAdmin);

        query = GetSortedQuery(sortDescending, query, sortBy);
        return await query.ToPagedListAsync(page, pageSize, cancel);
    }

    public async Task<bool> IsUserInOfficeAsync(int userId, int officeId, CancellationToken cancel = default)
    {
        return await _context.OfficeUsers.AsNoTracking().AnyAsync(x => x.UserId == userId && x.OfficeId == officeId && !x.IsDisabled, cancel);
    }

    private async Task<List<int>> GetUserOfficeIdsAsync(int userId, CancellationToken cancel = default)
    {
        return await _context.OfficeUsers
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.OfficeId)
            .Distinct()
            .ToListAsync(cancel);
    }

    private static IQueryable<OfficeUser> GetSortedQuery(bool sortDescending, IQueryable<OfficeUser> query, OfficeUsersSortColumn effectiveSortBy)
    {
        return effectiveSortBy switch
        {
            OfficeUsersSortColumn.Name => ApplyOrder(query, sortDescending, x => x.User!.FirstName),
            OfficeUsersSortColumn.Email => ApplyOrder(query, sortDescending, x => x.User!.Email),
            OfficeUsersSortColumn.LastUserLogin => ApplyOrder(query, sortDescending, x => x.User!.LastLoginDateTimeUtc),
            OfficeUsersSortColumn.Active => ApplyOrder(query, sortDescending, x => !x.IsDisabled),
            OfficeUsersSortColumn.Title => ApplyOrder(query, sortDescending, x => x.Title),
            OfficeUsersSortColumn.Role => ApplyOrder(query, sortDescending, x => x.Role!.Name),
            OfficeUsersSortColumn.LastUpdated => ApplyOrder(query, sortDescending, x => x.ModifyDateTimeUtc),
            _ => ApplyOrder(query, sortDescending, x => x.ModifyDateTimeUtc)
        };
    }

    private static IQueryable<OfficeUser> ApplyOrder<TKey>(IQueryable<OfficeUser> query, bool descending, System.Linq.Expressions.Expression<Func<OfficeUser, TKey>> keySelector)
    {
        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }
}
