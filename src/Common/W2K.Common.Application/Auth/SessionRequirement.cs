using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

/// <summary>
/// Authorization requirement that requires a valid X-Session-Id
/// and X-FingerPrint to be passed in the header
/// </summary>
public class SessionRequirement : IAuthorizationRequirement
{
}
