using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.Roles;

[ApiController]
[Route("api/v{version:apiVersion}/roles")]
public class RolesController : BaseApiController
{
    /// <summary>
    /// Gets active roles that can be assigned to a merchant office user.
    /// </summary>
    /// <returns>A list of roles.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewRoles)]
    public async Task<ActionResult<IList<RoleDto>>> GetRolesAsync()
    {
        var roles = await Mediator.Send(new GetRolesQuery(OfficeType.Merchant));
        return Ok(roles);
    }
}
