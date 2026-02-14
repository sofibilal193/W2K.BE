#pragma warning disable IDE0130 // Namespace does not match folder structure
using W2K.Common.Application.Auth;
using W2K.Identity.Repositories;

namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserPermissionsQueryHandler(IIdentityUnitOfWork data) : IRequestHandler<GetUserPermissionsQuery, IList<string>>
{
    private readonly IIdentityUnitOfWork _data = data;

    public async Task<IList<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        // return no permissions if no user is specified
        if (request.UserId is null)
        {
            return Array.Empty<string>();
        }

        // return permissions to allow new office enrollment if no office is specified
        if (request.OfficeId is null)
        {
            return [Permissions.ViewOffices, Permissions.UpdateOffice, Permissions.ViewOfficeOwner, Permissions.UpdateOfficeOwner, Permissions.ViewOfficeBankAccount, Permissions.UpdateOfficeBankAccount];
        }

        var user = await _data.Users.Include(x => x.Offices.Where(o => o.OfficeId == request.OfficeId && !o.IsDisabled))
            .Include("Offices.Role.Permissions")
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        return user?.Offices?.FirstOrDefault()?.Role?.Permissions?.Where(x => !x.IsDisabled).Select(x => x.Name).ToList() ?? [];
    }
}
