using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Entities;
using W2K.Identity.Application;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Enums;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.SuperAdmin;

public partial class SuperAdminController : BaseSuperAdminApiController
{

    /// <summary>
    /// Creates a new user for existing office and creates the office/user association.
    /// </summary>
    /// <param name="officeId">The ID of the existing office.</param>
    /// <param name="command">The command containing user details.</param>
    /// <returns>The ID of the created user.</returns>
    /// <response code="201">Returns the ID of the created user.</response>
    /// <response code="400">Validation error.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("office/{officeId}/user")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult<int?>> CreateOfficeUserAsync(int officeId, [FromBody] UpsertOfficeUserCommand command)
    {
        command.SetIds(officeId, null);
        var userId = await Mediator.Send(command);
        return Created("", new { UserId = userId });
    }

    /// <summary>
    /// Updates an existing user for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="command">The command containing updated user details.</param>
    /// <returns>No content.</returns>
    /// <response code="204">User updated successfully.</response>
    /// <response code="400">Validation error.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{officeId}/user/{userId}")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> UpdateOfficeUserAsync(int officeId, int userId, [FromBody] UpsertOfficeUserCommand command)
    {
        command.SetIds(officeId, userId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Send invite to existing user for new office.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="command">The command containing office and user details.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Office created and user associated successfully.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("users/{userId}/office")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> CreateNewOfficeForExistingUserAsync(int userId, [FromBody] UpsertOfficeUserCommand command)
    {
        command.SetIds(null, userId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets all office role assignments for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    [HttpGet("users/{userId}/officesRoles")]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<IList<UserOfficeRoleDto>>> GetOfficesRolesForUserAsync(int userId)
    {
        var result = await Mediator.Send(new GetOfficesRolesForUserQuery(userId));
        return Ok(result);
    }

    /// <summary>
    /// Upserts (adds/updates/deletes) office role assignments for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="command">The command containing office and user details.</param>
    /// <returns>No content.</returns>
    [HttpPut("users/{userId}/officesRoles")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> UpsertUserOfficesRolesAsync(int userId, UpsertUserOfficesRolesCommand command)
    {
        command.SetUserId(userId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Retrieves users for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <returns>A list of users for the specified office.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("office/{officeId}/users")]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<PagedList<OfficeUserDto>>> GetUsersForOfficeAsync(
        int officeId,
        int page = 1,
        int pageSize = 10,
        OfficeUsersSortColumn sortBy = OfficeUsersSortColumn.LastUpdated,
        bool sortDescending = true)
    {
        var users = await Mediator.Send(new GetOfficeUsersQuery(page, pageSize, officeId, sortBy, sortDescending, CleanseSensitiveFields: true));
        return Ok(users);
    }
}
