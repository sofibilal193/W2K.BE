using MediatR;

namespace W2K.Identity.Application.Commands.DeleteSuperAdminUser;

/// <summary>
/// Command to delete a SuperAdmin user from the system and Azure AD B2C.
/// </summary>
/// <param name="UserId">The ID of the user to delete.</param>
public record DeleteSuperAdminUserCommand(int UserId) : IRequest;
