using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Queries;
using W2K.Common.Identity;
using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using Microsoft.AspNetCore.Http;
using W2K.Common.Application.Auth;

namespace W2K.Identity.Controllers.SuperAdmin;

[ApiController]
public partial class SuperAdminController : BaseSuperAdminApiController
{
    /// <summary>
    /// Gets active roles that can be assigned to a SuperAdmin user.
    /// </summary>
    /// <returns>A list of SuperAdmin roles.</returns>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewRoles)]
    public async Task<ActionResult<IList<RoleDto>>> GetRolesAsync()
    {
        // Always fetch SuperAdmin roles only
        var roles = await Mediator.Send(new GetRolesQuery(OfficeType.SuperAdmin));
        return Ok(roles);
    }
}
