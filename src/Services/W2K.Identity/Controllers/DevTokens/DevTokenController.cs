using Asp.Versioning;
using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.DevTokens;

// A simple controller that issues JWT tokens in a DEV environment when running locally.
[AllowAnonymous]
[Route("api/v{version:apiVersion}/devtokens")]
[ApiVersion("1.0")]
public class DevTokenController : BaseApiController
{
    [HttpPost]
    [ApiConventionMethod(typeof(ApiConventions), nameof(ApiConventions.Post))]
    public async Task<ActionResult<ValidateDevUserResponse>> ValidateCredentialsAsync(ValidateDevUserCommand cmd)
    {
        var response = await Mediator.Send(cmd);
        return response == default ? (ActionResult<ValidateDevUserResponse>)Unauthorized() : (ActionResult<ValidateDevUserResponse>)Ok(response);
    }
}
