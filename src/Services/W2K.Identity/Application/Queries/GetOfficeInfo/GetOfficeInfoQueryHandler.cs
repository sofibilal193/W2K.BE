using AutoMapper;
using W2K.Common.Exceptions;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeInfoQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficeInfoQuery, OfficeInfoDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<OfficeInfoDto> Handle(GetOfficeInfoQuery query, CancellationToken cancellationToken)
    {
        var office = await _data.Offices.Include(x => x.Addresses)
                        .AsNoTracking()
                        .GetAsync(query.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Entities.Office), query.OfficeId);
        return _mapper.Map<OfficeInfoDto>(office);
    }
}
