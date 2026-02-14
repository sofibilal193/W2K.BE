using W2K.Common.Application.Auth;
using W2K.Common.Application.Filters;
using W2K.Common.Application.Settings;
using W2K.Common.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace W2K.Common.Application.Controllers;

#pragma warning disable CS9264 // Non-nullable property must contain a non-null value when exiting constructor

[ApiController]
[Authorize(AuthPolicies.SuperAdmin)]
[EnableCors]
[ApiConventionType(typeof(ApiConventions))]
[ServiceFilter(typeof(ValidateBadRequestCommandFilter))]
// https://github.com/AzureAD/microsoft-identity-web/wiki/web-apis#verification-of-scopes-or-app-roles-in-the-controller-actions
[Route("api/v{version:apiVersion}/superadmin")]
public abstract class BaseSuperAdminApiController : ControllerBase
{
    protected const string RoutePrefix = "~/api/v{version:apiVersion}/superadmin";

    private ILogger? _logger;

    protected static string? OperationId => System.Diagnostics.Activity.Current?.RootId;

    protected IMediator Mediator => field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IHostEnvironment HostEnv => field ??= HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();

    protected AppSettings AppSettings => field ??= HttpContext.RequestServices.GetRequiredService<IOptionsMonitor<AppSettings>>().CurrentValue;

    protected ICurrentUser CurrentUser => field ??= HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

    protected string BaseUrl => string.Format("{0}://{1}", Request.Scheme, Request.Host);

    protected ILogger GetLogger<T>()
        where T : class
    {
        return _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    }

}

#pragma warning restore CS9264 // Non-nullable property must contain a non-null value when exiting constructor
