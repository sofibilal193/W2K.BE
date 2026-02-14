using W2K.Identity.Application.DTOs;
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Query to return all office role assignments for a specific user (SuperAdmin scope).
/// </summary>
public readonly record struct GetOfficesRolesForUserQuery(int UserId) : IRequest<IList<UserOfficeRoleDto>>;
