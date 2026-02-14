using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using W2K.Config.Application.Queries;
using Microsoft.AspNetCore.Http;
using W2K.Config.Application.DTOs;
using W2K.Common.Application.Auth;
using Microsoft.AspNetCore.Authorization;
namespace W2K.Config.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/configs")]
public class ConfigsController : BaseApiController
{
    /// <summary>
    /// Gets the list of external config items.
    /// </summary>
    /// <returns>A list of config items (excluding internal and disabled), ordered by display order.</returns>
    /// <response code="200">Returns the list of config items</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("external")]
    [Authorize(AuthPolicies.TokenOrSession)]
    public async Task<ActionResult<IList<ConfigDto>>> GetConfigsAsync()
    {
        var result = await Mediator.Send(new GetExternalConfigsQuery());
        return Ok(result);
    }
}
