using DFI.Common.Application.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DFI.Common.Application.Auth;

public class SuperAdminUserRequirementHandler : AuthorizationHandler<SuperAdminUserRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminUserRequirement requirement)
    {
        if (context.User.IsSuperAdmin())
        {
            context.Succeed(requirement);
        }
        await Task.CompletedTask;
    }
}
