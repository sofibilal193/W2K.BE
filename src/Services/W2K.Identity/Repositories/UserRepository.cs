using W2K.Common.Entities;
using W2K.Common.Identity;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Application.Enums;
using W2K.Identity.Entities;
using W2K.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace W2K.Identity.Repositories;

public class UserRepository(IdentityDbContext context) : DbRepository<User>(context), IUserRepository
{
    private readonly IdentityDbContext _context = context;

    public Task<PagedList<User>> GetUserListAsync(
        int page,
        int pageSize,
        string? search,
        UserSortColumn sortBy,
        bool sortDescending,
        CancellationToken cancel = default)
    {
        // Fetch all Users, excluding any user who has SuperAdmin or Lender offices
        var query = _context.Users
            .AsNoTracking()
            .Include(x => x.Offices)
            .Where(x => !x.Offices.Any(o => o.Office != null && (o.Office.Type == OfficeType.SuperAdmin || o.Office.Type == OfficeType.Lender)))
            .AsQueryable();

        query = ApplySearchFilter(query, search);

        query = GetSortedQuery(sortDescending, query, sortBy);

        return query.ToPagedListAsync(page, pageSize, cancel);
    }

    public Task<PagedList<User>> GetSuperAdminUserListAsync(
        int page,
        int pageSize,
        string? search,
        UserSortColumn sortBy,
        bool sortDescending,
        CancellationToken cancel = default)
    {
        var query = _context.Users
            .AsNoTracking()
            .Include(x => x.Offices)
            .ThenInclude(x => x.Role)
            .Where(x => x.Offices.Any(o => o.Office != null && o.Office.Type == OfficeType.SuperAdmin))
            .AsQueryable();

        query = ApplySearchFilter(query, search);

        query = GetSortedQuery(sortDescending, query, sortBy);

        return query.ToPagedListAsync(page, pageSize, cancel);
    }

    public IAsyncEnumerable<User> SearchUsersForGlobalSearchAsync(
        int id,
        string searchTerm,
        bool isInteger)
    {
        return _context.Users
            .AsNoTracking()
            .Include(x => x.Offices)
            .ThenInclude(x => x.Office)
            .Where(x => !x.IsDisabled
                        && (isInteger
                            ? x.Id == id
                            : EF.Functions.Like(x.Email, searchTerm)
                                || EF.Functions.Like(x.FirstName ?? string.Empty, searchTerm)
                                || EF.Functions.Like(x.LastName ?? string.Empty, searchTerm)))
            .AsAsyncEnumerable();
    }

    private static IQueryable<User> ApplySearchFilter(IQueryable<User> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var searchPattern = $"%{search}%";
        return query.Where(x =>
            (x.FirstName != null && EF.Functions.Like(x.FirstName, searchPattern))
            || (x.LastName != null && EF.Functions.Like(x.LastName, searchPattern))
            || EF.Functions.Like(x.Email, searchPattern));
    }

    private static IQueryable<User> GetSortedQuery(bool sortDescending, IQueryable<User> query, UserSortColumn effectiveSortBy)
    {
        return effectiveSortBy switch
        {
            UserSortColumn.FirstName => ApplyOrder(query, sortDescending, x => x.FirstName),
            UserSortColumn.LastName => ApplyOrder(query, sortDescending, x => x.LastName),
            UserSortColumn.Email => ApplyOrder(query, sortDescending, x => x.Email),
            UserSortColumn.LastUserLogin => ApplyOrder(query, sortDescending, x => x.LastLoginDateTimeUtc),
            UserSortColumn.Active => ApplyOrder(query, sortDescending, x => !x.IsDisabled),
            UserSortColumn.LastUpdated => ApplyOrder(query, sortDescending, x => x.ModifyDateTimeUtc),
            _ => ApplyOrder(query, sortDescending, x => x.ModifyDateTimeUtc)
        };
    }

    private static IQueryable<User> ApplyOrder<TKey>(IQueryable<User> query, bool descending, System.Linq.Expressions.Expression<Func<User, TKey>> keySelector)
    {
        return descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }
}
