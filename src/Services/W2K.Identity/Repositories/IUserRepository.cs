using W2K.Common.Entities;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Application.Enums;
using W2K.Identity.Entities;

namespace W2K.Identity.Repositories;

public interface IUserRepository : IDbRepository<User>
{
    Task<PagedList<User>> GetUserListAsync(
        int page,
        int pageSize,
        string? search,
        UserSortColumn sortBy,
        bool sortDescending,
        CancellationToken cancel = default);

    Task<PagedList<User>> GetSuperAdminUserListAsync(
        int page,
        int pageSize,
        string? search,
        UserSortColumn sortBy,
        bool sortDescending,
        CancellationToken cancel = default);

    /// <summary>
    /// Searches users by ID, email, first name, or last name for global search
    /// </summary>
    /// <param name="id">Numeric ID to search for (exact match)</param>
    /// <param name="searchTerm">Partial search term (LIKE pattern)</param>
    /// <param name="isInteger">True if search term is a valid integer ID</param>
    /// <returns>Async enumerable of matching users with their offices</returns>
    /// <remarks>
    /// Use .WithCancellation(cancellationToken) when consuming this async enumerable to support cancellation.
    /// </remarks>
    IAsyncEnumerable<User> SearchUsersForGlobalSearchAsync(
        int id,
        string searchTerm,
        bool isInteger);
}
