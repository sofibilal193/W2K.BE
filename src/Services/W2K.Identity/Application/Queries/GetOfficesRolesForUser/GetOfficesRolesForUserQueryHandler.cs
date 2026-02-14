using AutoMapper;
using W2K.Common.Exceptions;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles retrieval of a user's office role assignments.
/// </summary>
public class GetOfficesRolesForUserQueryHandler(IIdentityUnitOfWork data, IMapper mapper)
    : IRequestHandler<GetOfficesRolesForUserQuery, IList<UserOfficeRoleDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<IList<UserOfficeRoleDto>> Handle(GetOfficesRolesForUserQuery request, CancellationToken cancellationToken)
    {
        var officeUsers = await _data.OfficeUsers.AsNoTracking()
                            .GetAsync(x => x.UserId == request.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(OfficeUser), request.UserId);

        return _mapper.Map<IList<UserOfficeRoleDto>>(officeUsers);
    }
}
