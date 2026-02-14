using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using W2K.Config.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using W2K.Common.Application.Auth;

namespace W2K.Config.Controllers;

[Route("api/v{version:apiVersion}/cache")]
public class CacheController : BaseApiController
{
    /// <summary>
    /// Clears all cache entries matching the specified pattern.
    /// </summary>
    /// <response code="204">Cache cleared successfully</response>
    /// <response code="401">Unauthorized - requires valid API key</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpDelete]
    [Authorize(AuthPolicies.ApiKey)]
    public async Task<ActionResult> ClearCacheAsync([FromBody] ClearCacheCommand command)
    {
        await Mediator.Send(command);
        return NoContent();
    }
}
