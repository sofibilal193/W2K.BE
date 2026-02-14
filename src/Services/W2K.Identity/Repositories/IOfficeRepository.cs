using W2K.Common.Entities;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Entities;
using W2K.Identity.Application.Enums;

namespace W2K.Identity.Repositories;

public interface IOfficeRepository : IDbRepository<Office>
{
    Task<List<int>> GetSuperAdminIdsAsync(CancellationToken cancel = default);

    Task<List<OfficeGroup>> GetGroupsAsync(CancellationToken cancel = default);

    Task<PagedList<Office>> GetPagedListAsync(int page, int pageSize, string? search, OfficeSortColumn sortBy, bool sortDescending, CancellationToken cancel = default);

    Task<PagedList<Office>> GetUserOfficeListAsync(int userId, int page, int pageSize, string? search, OfficeSortColumn sortBy, bool sortDescending, CancellationToken cancel = default);

    /// <summary>
    /// Searches offices by ID or name for global search
    /// </summary>
    /// <param name="id">Numeric ID to search for (exact match)</param>
    /// <param name="searchTerm">Partial name search term (LIKE pattern)</param>
    /// <param name="isInteger">True if search term is a valid integer ID</param>
    /// <returns>Async enumerable of matching offices</returns>
    /// <remarks>
    /// Use .WithCancellation(cancellationToken) when consuming this async enumerable to support cancellation.
    /// </remarks>
    IAsyncEnumerable<Office> SearchOfficesForGlobalSearchAsync(
        int id,
        string searchTerm,
        bool isInteger);
}
