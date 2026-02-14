using AutoMapper;
using W2K.Common.Entities;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserOfficesQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetUserOfficesQuery, PagedList<OfficeListItemDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PagedList<OfficeListItemDto>> Handle(GetUserOfficesQuery query, CancellationToken cancellationToken)
    {
        var offices = await _data.Offices.GetUserOfficeListAsync(
            _currentUser.UserId ?? 0,
            query.Page,
            query.PageSize,
            query.Search,
            query.SortBy,
            query.SortDescending,
            cancellationToken);

        return _mapper.Map<PagedList<OfficeListItemDto>>(offices);
    }
}
