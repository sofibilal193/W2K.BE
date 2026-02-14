using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Application.Auth;

/// <summary>
/// Routes authorization failures to appropriate HTTP status codes:
/// 401 for session failures, 403 for permission/office failures.
/// </summary>
public class SessionAuthorizationResultHandler(ILogger<SessionAuthorizationResultHandler> logger) : IAuthorizationMiddlewareResultHandler
{
    private static readonly IAuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();
    private readonly ILogger<SessionAuthorizationResultHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            await next(context);
            return;
        }

        var failure = authorizeResult.AuthorizationFailure;
        if (failure is null)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        // Check by handler type - session failures take priority over permission failures
        // This ensures expired/invalid sessions return 401, not 403
        if (failure.FailureReasons?.Any(x => x.Handler is SessionRequirementHandler) == true)
        {
            _logger.LogWarning("Authorization failed: Session validation failed.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (failure.FailureReasons?.Any(x => x.Handler is PermissionAuthorizationHandler or OfficeUserAuthorizationHandler) == true)
        {
            _logger.LogWarning("Authorization failed: Permission or office access denied.");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
