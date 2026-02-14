using W2K.Common.Application.Cacheing;
using W2K.Common.Identity;
using W2K.Identity.Extensions;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ValidateOfficeUserCommandHandler(IIdentityUnitOfWork data, ICache cache, ICurrentUser user) : IRequestHandler<ValidateOfficeUserCommand, bool>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ICache _cache = cache;
    private readonly ICurrentUser _user = user;

    public async Task<bool> Handle(ValidateOfficeUserCommand request, CancellationToken cancellationToken)
    {
        // check if user is member of org
        if (_user.OfficeIds is not null && _user.OfficeIds.Contains(request.OfficeId))
        {
            return true;
        }

        // check if user is member of any group within office's hierarchy
        var groups = await _cache.GetOfficeGroupsAsync(cancellationToken);
        if (groups is null)
        {
            groups = await _data.Offices.GetGroupsAsync(cancellationToken);
            await _cache.SetOfficeGroupsAsync(groups.AsReadOnly(), cancellationToken);
        }
        var officeIds = new List<int> { request.OfficeId };
        var parentOfficeIds = groups.Where(x => x.OfficeId == request.OfficeId).Select(x => x.ParentOfficeId).ToList();
        while (parentOfficeIds.Count > 0)
        {
            if (_user.OfficeIds is not null && _user.OfficeIds.Intersect(parentOfficeIds).Any())
            {
                return true;
            }
            officeIds.AddRange(parentOfficeIds);
            parentOfficeIds = [.. groups.Where(x => parentOfficeIds.Contains(x.OfficeId)).Select(x => x.ParentOfficeId)];
        }

        // check if user was added to office or group since last log in and OfficeId is not in claim
        var adminIds = await _cache.GetSuperAdminOfficeIdsAsync(cancellationToken);
        if (adminIds is null)
        {
            adminIds = await _data.Offices.GetSuperAdminIdsAsync(cancellationToken);
            await _cache.SetSuperAdminOfficeIdsAsync(adminIds.AsReadOnly(), cancellationToken);
        }
        officeIds.AddRange(adminIds);

        // check if user is super admin by office ids
        if (_user.OfficeIds is not null && _user.OfficeIds.Intersect(adminIds).Any())
        {
            return true;
        }

        // check if user was added to office since last login (only if userId is available)
        return _user.UserId is not null && await _data.OfficeUsers.IsUserInOfficeAsync(_user.UserId.Value, request.OfficeId, cancellationToken);
    }
}
