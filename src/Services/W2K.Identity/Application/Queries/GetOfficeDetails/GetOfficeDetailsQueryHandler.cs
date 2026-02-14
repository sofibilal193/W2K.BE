using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Common.Exceptions;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeDetailsQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficeDetailsQuery, OfficeDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<OfficeDto> Handle(GetOfficeDetailsQuery query, CancellationToken cancellationToken)
    {
        var office = await _data.Offices
                        .AsNoTracking()
                        .Include(x => x.Addresses)
                        .Include(x => x.Owners)
                        .Include(x => x.BankAccounts)
                        .FirstOrDefaultAsync(x => x.Id == query.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Office), query.OfficeId);

        return _mapper.Map<OfficeDto>(office);
    }
}
