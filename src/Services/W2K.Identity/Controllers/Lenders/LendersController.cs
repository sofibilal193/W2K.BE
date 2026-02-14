using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using Microsoft.AspNetCore.Http;
using W2K.Identity.Application;
using W2K.Common.Application;
using W2K.Common.Application.Auth;
using Microsoft.AspNetCore.Authorization;

namespace W2K.Identity.Controllers.Lenders;

[ApiController]
[Route("api/v{version:apiVersion}/lenders")]
public class LendersController : BaseApiController
{
    /// <summary>
    /// Gets all Lenders.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(AuthPolicies.TokenOrSessionOrApiKey)]
    public async Task<ActionResult<List<LenderDto>>> GetLendersAsync()
    {
        var lenders = await Mediator.Send(new GetLendersQuery());
        return Ok(lenders);
    }
}
