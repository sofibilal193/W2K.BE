using AutoMapper;
using W2K.Common.Entities;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeUsersQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetOfficeUsersQuery, PagedList<OfficeUserDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PagedList<OfficeUserDto>> Handle(GetOfficeUsersQuery query, CancellationToken cancellationToken)
    {
        var officeUsers = await _data.OfficeUsers.GetOfficeUsersAsync(query.Page, query.PageSize, _currentUser.UserId!.Value, query.OfficeId, query!.SortBy, query.SortDescending, cancellationToken);

        var result = _mapper.Map<PagedList<OfficeUserDto>>(officeUsers);

        // Cleanse sensitive fields if requested by the caller
        return query.CleanseSensitiveFields
            ? result with { Items = [.. result.Items.Select(x => x.Cleanse())] }
            : result;
    }
}
