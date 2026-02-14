using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using Microsoft.AspNetCore.Http;
using W2K.Identity.Application.Queries;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.DTOs;
using W2K.Common.Application.Auth;
using W2K.Identity.Application.Enums;
using W2K.Common.Entities;

namespace W2K.Identity.Controllers.OfficeUsers;

[ApiController]
[Route("api/v{version:apiVersion}/offices/{officeId}/users")]
public class OfficeUsersController : BaseApiController
{
    /// <summary>
    /// Retrieves users for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="sortBy">The field to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <returns>A list of users for the specified office.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<PagedList<OfficeUserDto>>> GetUsersForOfficeAsync(
        int officeId,
        int page = 1,
        int pageSize = 10,
        OfficeUsersSortColumn sortBy = OfficeUsersSortColumn.LastUpdated,
        bool sortDescending = true)
    {
        var users = await Mediator.Send(new GetOfficeUsersQuery(page, pageSize, officeId, sortBy, sortDescending));
        return Ok(users);
    }

    /// <summary>
    /// Create a new user and adds it to a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing user details.</param>
    /// <returns>The ID of the created user.</returns>
    /// <response code="201">Returns the ID of the created user</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult<int?>> CreateOfficeUserAsync(int officeId, [FromBody] UpsertOfficeUserCommand command)
    {
        command.SetIds(officeId, null);
        var userId = await Mediator.Send(command);
        return Created("", new { UserId = userId });
    }

    /// <summary>
    /// Updates an existing user in a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="command">The command containing updated user details.</param>
    /// <returns>No content response indicating the update was successful.</returns>
    [HttpPut("{userId}")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> UpdateOfficeUserAsync(int officeId, int userId, [FromBody] UpsertOfficeUserCommand command)
    {
        command.SetIds(officeId, userId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Delete a specific user from an office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>No content if the user was successfully deleted.</returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{userId}")]
    [HasPermission(Common.Application.Auth.Permissions.DeleteUser)]
    public async Task<ActionResult> DeleteOfficeUserAsync(int officeId, int userId)
    {
        await Mediator.Send(new DeleteOfficeUserCommand(officeId, userId));
        return NoContent();
    }

    /// <summary>
    /// Activates a user for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="userId">The ID of the user to activate.</param>
    [HttpPut("{userId}/activate")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> ActivateUserAsync(int officeId, int userId)
    {
        await Mediator.Send(new ActivateUserCommand(officeId, userId, true));
        return NoContent();
    }

    /// <summary>
    /// Deactivates a user for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="userId">The ID of the user to deactivate.</param>
    [HttpPut("{userId}/deactivate")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> DeactivateUserAsync(int officeId, int userId)
    {
        await Mediator.Send(new ActivateUserCommand(officeId, userId, false));
        return NoContent();
    }
}
