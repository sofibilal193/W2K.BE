using AutoMapper;
using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficesForSuperAdminQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficesForSuperAdminQuery, PagedList<OfficeListItemDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<PagedList<OfficeListItemDto>> Handle(GetOfficesForSuperAdminQuery query, CancellationToken cancellationToken)
    {
        var pagedOffices = await _data.Offices.GetPagedListAsync(
            query.Page,
            query.PageSize,
            query.Search,
            query.SortBy,
            query.SortDescending,
            cancellationToken);

        return _mapper.Map<PagedList<OfficeListItemDto>>(pagedOffices);
    }
}
