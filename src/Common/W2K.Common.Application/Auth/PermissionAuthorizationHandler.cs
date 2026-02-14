using System.Security.Claims;
using W2K.Common.Application.ApiClients;
using W2K.Common.Application.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Application.Auth;

public class PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IIdentityApiClient identityApiClient, ILogger<PermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IIdentityApiClient _identityApiClient = identityApiClient;
    private readonly ILogger<PermissionAuthorizationHandler> _logger = logger;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return;
        }

        // only check permissions for SuperAdmin user if the endpoint has SuperAdminUserRequirement
        if (requirement.Permissions.Count == 0 || (context.User.IsSuperAdmin() && !context.Requirements.Any(x => x is SuperAdminUserRequirement)))
        {
            context.Succeed(requirement);
            return;
        }

        // Skip permission checks if the user is authenticated with session-based authentication
        if (context.User.IsSessionAuthenticated())
        {
            context.Succeed(requirement);
            return;
        }

        try
        {
            var officeId = GetOfficeId(context.User);
            var userId = context.User.GetUserId();
            var permissions = await _identityApiClient.GetUserPermissionsAsync(userId, officeId);
            _logger.LogInformation("Retrieved Permissions: {Permissions} for User: {UserId} and Office: {OfficeId}.", permissions, userId, officeId);
            if (permissions?.Intersect(requirement.Permissions).Any() == true)
            {
                context.Succeed(requirement);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while authorizing permissions.");
        }

        context.Fail();
    }

    private int? GetOfficeId(ClaimsPrincipal user)
    {
        if (user.IsSuperAdmin())
        {
            return user.GetDefaultOfficeId();
        }
        var routeValues = _httpContextAccessor.HttpContext!.Request.RouteValues;
        if (routeValues.TryGetValue(AuthConstants.OfficeIdRouteValue, out var officeIdParam) && int.TryParse(officeIdParam?.ToString(), out var officeIdFromRoute))
        {
            return officeIdFromRoute;
        }

        var query = _httpContextAccessor.HttpContext!.Request.Query;
        if (query.TryGetValue(AuthConstants.OfficeIdRouteValue, out var officeIdQueryParam) && int.TryParse(officeIdQueryParam.ToString(), out var officeIdFromQuery))
        {
            return officeIdFromQuery;
        }

        // If no officeId found in route or query parameters, use the user's default office ID as fallback
        return user.GetDefaultOfficeId();
    }
}

