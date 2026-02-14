using W2K.Common.Application.Controllers;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.Permissions;

[ApiController]
[Route("api/v{version:apiVersion}/permissions")]
public class PermissionsController : BaseApiController
{
    /// <summary>
    /// Gets all permissions for an user.
    /// </summary>
    /// <param name="userId">ID of user.</param>
    /// <param name="officeId">ID of office.</param>
    /// <returns>List of permissions.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetUserPermissionsAsync([FromQuery] int? userId, [FromQuery] int? officeId)
    {
        var permissions = await Mediator.Send(new GetUserPermissionsQuery(userId, officeId));
        return Ok(permissions);
    }

}
