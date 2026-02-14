using AutoMapper;
using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUsersQueryHandler(IIdentityUnitOfWork data, IMapper mapper) :
 IRequestHandler<GetUsersQuery, PagedList<UserListItemDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<PagedList<UserListItemDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _data.Users.GetUserListAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.SortDescending,
            cancellationToken);

        return _mapper.Map<PagedList<UserListItemDto>>(users);
    }
}
