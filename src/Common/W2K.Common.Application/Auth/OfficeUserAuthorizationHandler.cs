using System.Security.Claims;
using DFI.Common.Application.ApiClients;
using DFI.Common.Application.Identity;
using DFI.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace DFI.Common.Application.Auth;

public class OfficeUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IIdentityApiClient identityApiClient) : AuthorizationHandler<OfficeUserRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IIdentityApiClient _identityApiClient = identityApiClient;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OfficeUserRequirement requirement)
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        var routeValues = httpContext.Request.RouteValues;

        if (routeValues.Values.Intersect(AuthConstants.ValidateOfficeUserKeyValues).Any())
        {
            context.Succeed(requirement);
            return;
        }

        // Skip permission checks if the user is authenticated with session-based authentication
        if (context.User.IsSessionAuthenticated() || context.User.IsApiKeyAuthenticated())
        {
            context.Succeed(requirement);
            return;
        }

        if (!TryGetOfficeIdParam(httpContext, routeValues, out var officeIdParam))
        {
            context.Succeed(requirement);
            return;
        }

        var officeId = ParseOfficeIdOrThrow(officeIdParam!, routeValues);

        if (UserHasDirectOfficeAccess(context, officeId))
        {
            context.Succeed(requirement);
            return;
        }

        if (!ShouldCheckApiForAccess(context))
        {
            context.Succeed(requirement);
            return;
        }

        bool hasAccess = await CheckOfficeAccessViaApiAsync(officeId);
        if (!hasAccess)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private static bool TryGetOfficeIdParam(HttpContext httpContext, IDictionary<string, object?> routeValues, out object? officeIdParam)
    {
        officeIdParam = null;

        if (routeValues.TryGetValue(AuthConstants.OfficeIdRouteValue, out var routeOfficeId) && routeOfficeId is not null)
        {
            officeIdParam = routeOfficeId;
            return true;
        }

        var query = httpContext.Request.Query;
        if (query.TryGetValue(AuthConstants.OfficeIdRouteValue, out var qv) && qv.Count > 0 && !string.IsNullOrWhiteSpace(qv[0]))
        {
            officeIdParam = qv[0];
            return true;
        }

        return false;
    }

    private static int ParseOfficeIdOrThrow(object officeIdParam, IDictionary<string, object?> routeValues)
    {
        return int.TryParse(officeIdParam?.ToString(), out var officeId) || routeValues.Values.Contains("enableOffice")
            ? officeId
            : throw new NotFoundException();
    }

    private static bool UserHasDirectOfficeAccess(AuthorizationHandlerContext context, int officeId)
    {
        return context.User.IsSuperAdmin() || context.User.IsMemberOfOffice(officeId);
    }

    private bool ShouldCheckApiForAccess(AuthorizationHandlerContext context)
    {
        return _httpContextAccessor.HttpContext is not null
            && !_httpContextAccessor.HttpContext.IsApiKeyAuth()
            && context.User.ContainsOfficeIdClaim()
            && context.User.ContainsUserIdClaim();
    }

    private async Task<bool> CheckOfficeAccessViaApiAsync(int officeId)
    {
        return await _identityApiClient.ValidateOfficeUserAsync(officeId);
    }
}
