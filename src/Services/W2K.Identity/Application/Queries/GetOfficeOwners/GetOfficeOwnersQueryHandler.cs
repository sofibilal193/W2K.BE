using AutoMapper;
using W2K.Common.Application.DTOs;
using W2K.Common.Identity;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeOwnersQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) :
IRequestHandler<GetOfficeOwnersQuery, List<OfficeOwnerDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<List<OfficeOwnerDto>> Handle(GetOfficeOwnersQuery query, CancellationToken cancellationToken)
    {
        var officeOwners = (await _data.Offices.Include(x => x.Owners).FirstOrDefaultAsync(
            x => x.Id == query.OfficeId
                    && (!x.IsDisabled || _currentUser.OfficeType == OfficeType.SuperAdmin),
            cancellationToken))
                ?.Owners.ToList();

        return _mapper.Map<List<OfficeOwnerDto>>(officeOwners);
    }
}
