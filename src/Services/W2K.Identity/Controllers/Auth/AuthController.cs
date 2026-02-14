using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Application.Filters;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace W2K.Identity.Controllers.Auth;

[ApiController]
[EnableCors]
[ApiConventionType(typeof(ApiConventions))]
[ServiceFilter(typeof(ValidateBadRequestCommandFilter))]
[Route("api/v{version:apiVersion}/auth")]
[ApiExplorerSettings(IgnoreApi = true)]
public class AuthController : ControllerBase
{
    private ILogger<AuthController> Logger => field
        ??= HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();

    private IMediator Mediator => field ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    [HttpPost("userInfo")]
    [Authorize(AuthPolicies.Basic)]
    public async Task<ActionResult<ResponseContent>> GetUserInfo([FromBody] AzureAdB2CRequest request)
    {
        ResponseContent? response = null;
        Logger.LogInformation("GetUserInfo.Request: {Data}", request);

        if (!string.IsNullOrEmpty(request?.Id))
        {
            var info = await Mediator.Send(new GetLoginUserInfoQuery
            {
                ProviderId = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = request.DisplayName,
                Email = request.Email,
                MobilePhone = request.MobilePhone,
                Step = request.Step
            });

            if (!string.IsNullOrEmpty(info.FirstName)
                && !string.IsNullOrEmpty(info.LastName))
            {
                Logger.LogInformation("Found UserId: {Id}", info.Id);
                response = new ResponseContent
                {
                    Id = info.Id,
                    FirstName = info.FirstName,
                    LastName = info.LastName,
                    Email = info.Email,
                    MobilePhone = info.MobilePhone,
                    OfficeId = info.OfficeId,
                    OfficeType = info.OfficeType,
                    OfficeName = info.OfficeName,
                    OfficeRole = info.OfficeRole,
                    OfficeTitle = info.OfficeTitle,
                    OfficePermissions = string.Join('|', info.OfficePermissions ?? []),
                    OfficeIds = string.Join('|', info.OfficeIds),
                    IsMultipleOffices = info.IsMultipleOffices,
                    IsUserDisabled = info.IsUserDisabled,
                    IsOfficeUserDisabled = info.IsOfficeUserDisabled,
                    IsSuperAdmin = info.IsSuperAdmin,
                    IsEnrollmentCompleted = info.IsEnrollmentCompleted
                };
            }
        }

        response ??= new ResponseContent
        {
            FirstName = request?.FirstName,
            LastName = request?.LastName
        };

        Logger.LogInformation("GetUserInfo.Response: {Data}", response);

        return Ok(response);
    }

    /// <summary>
    /// Gets whether the currently logged in user has access to an office.
    /// </summary>
    /// <param name="officeId">Id of Office.</param>
    [HttpGet("officeAccess")]
    [Authorize]
    public async Task<ActionResult<bool>> ValidateOfficeUserAsync(int officeId)
    {
        var hasAccess = await Mediator.Send(new ValidateOfficeUserCommand(officeId));
        return Ok(hasAccess);
    }
}
