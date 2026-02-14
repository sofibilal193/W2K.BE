using W2K.Common.Application.Identity;
using Microsoft.AspNetCore.Authorization;

namespace W2K.Common.Application.Auth;

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
