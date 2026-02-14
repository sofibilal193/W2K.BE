using Microsoft.AspNetCore.Authorization;

namespace W2K.Common.Application.Auth;

/// <summary>
/// Authorization requirement that requires a user is SuperAdmin
/// </summary>
public class SuperAdminUserRequirement : IAuthorizationRequirement
{
}
