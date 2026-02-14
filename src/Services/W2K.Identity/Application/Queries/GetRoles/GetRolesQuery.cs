using W2K.Identity.Application.DTOs;
using W2K.Common.Identity;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record GetRolesQuery(OfficeType OfficeType) : IRequest<IList<RoleDto>>;
