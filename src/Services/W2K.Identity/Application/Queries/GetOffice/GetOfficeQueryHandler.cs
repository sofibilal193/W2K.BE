using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficeQuery, OfficeDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<OfficeDto> Handle(GetOfficeQuery query, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.AsNoTracking().GetAsync(query.OfficeId, cancellationToken);
        return _mapper.Map<OfficeDto>(office);
    }
}
