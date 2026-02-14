using AutoMapper;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handler for retrieving office enrollment details.
/// </summary>
public class GetOfficeEnrollmentDetailQueryHandler(
    IIdentityUnitOfWork data,
    IMapper mapper)
    : IRequestHandler<GetOfficeEnrollmentDetailQuery, OfficeEnrollmentDetailDto>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OfficeEnrollmentDetailDto> Handle(GetOfficeEnrollmentDetailQuery query, CancellationToken cancellationToken)
    {
        var office = await _data.Offices
                        .Include(x => x.Addresses)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x => x.Id == query.OfficeId
                                    && !(x.Type == OfficeType.SuperAdmin || x.Type == OfficeType.Lender),
                            cancellationToken)
            ?? throw new NotFoundException(nameof(Office), query.OfficeId);

        return _mapper.Map<OfficeEnrollmentDetailDto>(office);
    }
}
