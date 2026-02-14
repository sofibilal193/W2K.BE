using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

/// <summary>
/// Authorization requirement that requires a user to belong to an
/// office in order to access data for that office.
/// </summary>
public class OfficeUserRequirement : IAuthorizationRequirement
{
}
