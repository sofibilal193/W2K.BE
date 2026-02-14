using W2K.Common.Entities;
using W2K.Common.Identity;
using W2K.Common.Persistence.Extensions;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Entities;
using W2K.Identity.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using W2K.Identity.Application.Enums;

namespace W2K.Identity.Repositories;

public class OfficeRepository(IdentityDbContext context) : DbRepository<Office>(context), IOfficeRepository
{
    // Centralized allowed sort columns for maintainability
    private static readonly OfficeSortColumn[] AllowedSortColumnsForPagedList =
    [
        OfficeSortColumn.Id,
        OfficeSortColumn.Name,
        OfficeSortColumn.LastUpdated,
        OfficeSortColumn.LastUserLogin,
        OfficeSortColumn.Active,
        OfficeSortColumn.EnrollmentCompleted,
        OfficeSortColumn.Approved
    ];

    private static readonly OfficeSortColumn[] AllowedSortColumnsForUserOfficeList =
    [
        OfficeSortColumn.Id,
        OfficeSortColumn.Name,
        OfficeSortColumn.Active,
        OfficeSortColumn.EnrollmentCompleted,
        OfficeSortColumn.Approved
    ];

    private readonly IdentityDbContext _context = context;

    public async Task<List<int>> GetSuperAdminIdsAsync(CancellationToken cancel = default)
    {
        return await _context.Offices
            .AsNoTracking()
            .Where(x => !x.IsDisabled && x.Type == OfficeType.SuperAdmin)
            .Select(x => x.Id)
            .ToListAsync(cancel);
    }

    public async Task<List<OfficeGroup>> GetGroupsAsync(CancellationToken cancel = default)
    {
        return await _context.Offices
            .AsNoTracking()
            .Include(x => x.Groups)
            .Where(x => x.Type != OfficeType.SuperAdmin && !x.IsDisabled)
            .SelectMany(x => x.Groups)
            .ToListAsync(cancel);
    }

    public async Task<PagedList<Office>> GetPagedListAsync(int page, int pageSize, string? search, OfficeSortColumn sortBy, bool sortDescending, CancellationToken cancel = default)
    {
        var query = _context.Offices
            .Include(x => x.Addresses)
            .Where(
                x => x.Type != OfficeType.SuperAdmin
                        && x.Type != OfficeType.Lender)
            .AsNoTracking()
            .AsQueryable();

        query = BuildOfficeQuery(query, search, sortBy, sortDescending, AllowedSortColumnsForPagedList, OfficeSortColumn.LastUpdated);
        return await query.ToPagedListAsync(page, pageSize, cancel);
    }

    public async Task<PagedList<Office>> GetUserOfficeListAsync(
        int userId,
        int page,
        int pageSize,
        string? search,
        OfficeSortColumn sortBy,
        bool sortDescending,
        CancellationToken cancel = default)
    {
        var officeQuery = _context.Offices
            .Include(x => x.Addresses)
            .AsNoTracking()
            .Where(x => x.Type != OfficeType.SuperAdmin
                        && x.Users.Any(ou => ou.UserId == userId && ou.IsDisabled == x.IsDisabled)); // Show only when Office.IsDisabled matches OfficeUser.IsDisabled

        officeQuery = BuildOfficeQuery(officeQuery, search, sortBy, sortDescending, AllowedSortColumnsForUserOfficeList, OfficeSortColumn.Id);
        return await officeQuery.ToPagedListAsync(page, pageSize, cancel);
    }

    public IAsyncEnumerable<Office> SearchOfficesForGlobalSearchAsync(
        int id,
        string searchTerm,
        bool isInteger)
    {
        return _context.Offices
            .AsNoTracking()
            .Where(x => !x.IsDisabled
                        && x.Type != OfficeType.SuperAdmin
                        && x.Type != OfficeType.Lender
                        && (isInteger ? x.Id == id : EF.Functions.Like(x.Name, searchTerm)))
            .AsAsyncEnumerable();
    }

    private static IQueryable<Office> BuildOfficeQuery(
    IQueryable<Office> query,
    string? search,
    OfficeSortColumn sortBy,
    bool sortDescending,
    OfficeSortColumn[] allowedSortColumns,
    OfficeSortColumn defaultSortColumn)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = int.TryParse(search, out var id)
                ? query.Where(x => x.Id == id)
                : query.Where(x => x.Name.Contains(search));
        }

        // Only allow sorting by allowed columns
        var effectiveSortBy = allowedSortColumns.Contains(sortBy) ? sortBy : defaultSortColumn;

        var sortColumn = effectiveSortBy switch
        {
            OfficeSortColumn.Id => nameof(Office.Id),
            OfficeSortColumn.Name => nameof(Office.Name),
            OfficeSortColumn.LastUpdated => nameof(Office.ModifyDateTimeUtc),
            OfficeSortColumn.LastUserLogin => nameof(Office.LastLoginDateTimeUtc),
            OfficeSortColumn.Active => nameof(Office.IsDisabled),
            OfficeSortColumn.EnrollmentCompleted => nameof(Office.IsEnrollmentCompleted),
            OfficeSortColumn.Approved => nameof(Office.IsApproved),
            _ => nameof(Office.ModifyDateTimeUtc)
        };

        var sortExpression = sortDescending ? $"-{sortColumn}" : sortColumn;
        return query.OrderBy(sortExpression);
    }
}
