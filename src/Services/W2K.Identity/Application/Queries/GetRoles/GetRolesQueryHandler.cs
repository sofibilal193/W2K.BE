using AutoMapper;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetRolesQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetRolesQuery, IList<RoleDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<IList<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _data.Roles.GetAsync(x => !x.IsDisabled && x.OfficeType == request.OfficeType, cancellationToken);
        var dto = _mapper.Map<IList<RoleDto>>(roles);
        return dto;
    }
}
