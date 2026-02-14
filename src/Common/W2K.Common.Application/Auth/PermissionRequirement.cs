using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

/// <summary>
/// Authorization requirement that requires a user to
/// have one or more permissions.
/// </summary>
public class PermissionRequirement(IEnumerable<string> permissions) : IAuthorizationRequirement
{
    public ReadOnlyCollection<string> Permissions { get; init; } = new ReadOnlyCollection<string>([.. permissions]);
}
