using AutoMapper;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class SearchUsersQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<SearchUsersQuery, IList<UserDetailDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IList<UserDetailDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var emailSearchTerm = $"%{request.Email.Trim().ToLowerInvariant()}%";

        var users = await _data.Users.AsNoTracking()
            .GetAsync(
                x => !x.IsDisabled  // Filter disabled USERS
                        && EF.Functions.Like(x.Email, emailSearchTerm)
                        && (_currentUser.OfficeType == OfficeType.SuperAdmin
                            || x.Offices.Any(o => !o.IsDisabled && _currentUser.OfficeIds!.Contains(o.OfficeId))),
                cancellationToken);

        return _mapper.Map<IList<UserDetailDto>>(users);
    }
}
