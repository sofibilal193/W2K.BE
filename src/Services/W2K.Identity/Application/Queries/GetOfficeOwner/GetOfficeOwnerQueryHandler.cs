using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Common.Exceptions;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeOwnerQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficeOwnerQuery, OfficeOwnerDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<OfficeOwnerDto> Handle(GetOfficeOwnerQuery query, CancellationToken cancellationToken)
    {
        var office = await _data.Offices
                        .AsNoTracking()
                        .Include(x => x.Owners.Where(
                                        x => x.FirstName == query.FirstName
                                                && x.LastName == query.LastName
                                                && (x.Email == query.Email || x.MobilePhone == query.MobilePhone)))
                        .GetAsync(query.OfficeId, cancellationToken)
            ?? throw new NotFoundException(nameof(Office), query.OfficeId);

        return _mapper.Map<OfficeOwnerDto>(office.Owners.FirstOrDefault());
    }
}
