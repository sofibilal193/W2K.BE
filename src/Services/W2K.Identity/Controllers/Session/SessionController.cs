using Asp.Versioning;
using W2K.Common.Application.Controllers;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.Session;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/session")]
public class SessionController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSessionDto?>> UpsertSessionAsync([FromBody] UpsertSessionCommand command)
    {
        var session = await Mediator.Send(command);
        return Ok(session);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAsync()
    {
        await Mediator.Send(new LogoutSessionCommand());
        return NoContent();
    }
}
