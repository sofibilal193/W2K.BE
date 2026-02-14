using W2K.Common.Identity;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeOwnersListQueryHandler(IIdentityUnitOfWork data, ICurrentUser currentUser) : IRequestHandler<GetOfficeOwnersListQuery, List<OfficesOwnersListItemDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<List<OfficesOwnersListItemDto>> Handle(GetOfficeOwnersListQuery query, CancellationToken cancellationToken)
    {
        var officeOwners = (await _data.Offices
            .Include(x => x.Owners)
            .GetAsync(x => !x.IsDisabled && x.Users.Any(u => u.UserId == _currentUser.UserId), cancellationToken))
            .SelectMany(
                x => x.Owners.Select(
                        owner =>
                        new OfficesOwnersListItemDto(
                            x.Id,
                            x.Name,
                            owner.FirstName,
                            owner.LastName,
                            owner.Email,
                            owner.MobilePhone)))
            .Distinct()
            .ToList(); return officeOwners;
    }
}
