using W2K.Common.Entities;
using W2K.Common.Persistence.Repositories;
using W2K.Identity.Application.Enums;
using W2K.Identity.Entities;

namespace W2K.Identity.Repositories;

public interface IOfficeUserRepository : IDbRepository<OfficeUser>
{
    Task<PagedList<OfficeUser>> GetOfficeUsersAsync(int page, int pageSize, int userId, int? officeId, OfficeUsersSortColumn sortBy, bool sortDescending, CancellationToken cancel = default);

    Task<bool> IsUserInOfficeAsync(int userId, int officeId, CancellationToken cancel = default);
}
