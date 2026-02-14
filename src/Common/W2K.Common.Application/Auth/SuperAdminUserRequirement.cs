using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

/// <summary>
/// Authorization requirement that requires a user is SuperAdmin
/// </summary>
public class SuperAdminUserRequirement : IAuthorizationRequirement
{
}
