using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Entities;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.Commands.DeleteSuperAdminUser;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Enums;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.SuperAdmin;

public partial class SuperAdminController : BaseSuperAdminApiController
{
    /// <summary>
    /// Search users by email for autocomplete.
    /// </summary>
    /// <param name="email">Email to search (supports partial values).</param>
    /// <returns>List of matching users with their details</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<IList<UserDetailDto>>> SearchUsersAsync([FromQuery] string email)
    {
        var result = await Mediator.Send(new SearchUsersQuery(email));
        return Ok(result);
    }

    /// <summary>
    /// Gets paged list of unique users with search and sort.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">>Number of items per page</param>
    /// <param name="search">Search term for email or name.</param>
    /// <param name="sortBy">Sort by field (email, name).</param>
    /// <param name="sortDescending">Sort descending.</param>
    /// <returns>Paged list of unique users.</returns>
    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<PagedList<UserListItemDto>>> GetUsersAsync(int page, int pageSize, string? search, UserSortColumn sortBy = UserSortColumn.LastUpdated, bool sortDescending = true)
    {
        var result = await Mediator.Send(new GetUsersQuery(page, pageSize, search, sortBy, sortDescending));
        return Ok(result);
    }

    /// <summary>
    /// Gets paged list of SuperAdmin users with search and sort.
    /// </summary>
    /// <param name="search">Search term for email or name.</param>
    /// <param name="sortBy">Sort by field (email, name).</param>
    /// <param name="sortDescending">Sort descending.</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paged list of SuperAdmin users.</returns>
    [HttpGet("superadmin-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewSuperAdminUsers)]
    public async Task<ActionResult<PagedList<SuperAdminUserListItemDto>>> GetSuperAdminUsersAsync(int page, int pageSize, string? search, UserSortColumn sortBy = UserSortColumn.LastUpdated, bool sortDescending = true)
    {
        var result = await Mediator.Send(new GetSuperAdminUsersQuery(page, pageSize, search, sortBy, sortDescending));
        return Ok(result);
    }

    /// <summary>
    /// Creates a new SuperAdmin user.
    /// </summary>
    /// <param name="command">The command containing SuperAdmin user details.</param>
    /// <returns>The ID of the created SuperAdmin user.</returns>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [HttpPost("superadmin-user")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateSuperAdminUsers)]
    public async Task<ActionResult<int?>> CreateSuperAdminUserAsync([FromBody] UpsertSuperAdminUserCommand command)
    {
        var userId = await Mediator.Send(command);
        return Created("", new { UserId = userId });
    }

    /// <summary>
    /// Updates an existing SuperAdmin user.
    /// </summary>
    /// <param name="userId">The ID of the SuperAdmin user to update.</param>
    /// <param name="command">The command containing updated SuperAdmin user details.</param>
    /// <returns>No content response indicating the update was successful.</returns>
    [HttpPut("superadmin-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateSuperAdminUsers)]
    public async Task<ActionResult> UpdateSuperAdminUserAsync([FromRoute] int userId, [FromBody] UpsertSuperAdminUserCommand command)
    {
        command.SetUserId(userId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a SuperAdmin user from the system and Azure AD B2C.
    /// </summary>
    /// <param name="userId">The ID of the SuperAdmin user to delete.</param>
    /// <returns>No content response indicating the deletion was successful.</returns>
    [HttpDelete("superadmin-users/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HasPermission(Common.Application.Auth.Permissions.DeleteSuperAdminUsers)]
    public async Task<ActionResult> DeleteSuperAdminUserAsync([FromRoute] int userId)
    {
        await Mediator.Send(new DeleteSuperAdminUserCommand(userId));
        return NoContent();
    }

    /// <summary>
    /// Returns detailed information for a user including what offices the user is associated with.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Comprehensive user details.</returns>
    [HttpGet("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewUsers)]
    public async Task<ActionResult<UserDetailsDto>> GetUserDetailsAsync(int userId)
    {
        var result = await Mediator.Send(new GetUserDetailsQuery(userId));
        return Ok(result);
    }

    /// <summary>
    /// Gets all user-accessible offices for the logged-in SuperAdmin user.
    /// Returns all office-user associations across the system, excluding SuperAdmin and Lender office types.
    /// </summary>
    /// <returns>List of office-user associations accessible to SuperAdmin, excluding SuperAdmin and Lender office types.</returns>
    /// <response code="200">Returns the list of user-accessible office information.</response>
    [HttpGet("users/offices")]
    [ProducesResponseType(typeof(IList<UserOfficeListItemDto>), StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<IList<UserOfficeListItemDto>>> GetOfficeListForLoggedInUserAsync()
    {
        var result = await Mediator.Send(new GetUserOfficeListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="command">The command containing user details and optional new office id.</param>
    /// <returns>The ID of the created user.</returns>
    /// <response code="201">Returns the ID of the created user.</response>
    /// <response code="400">Validation error.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("users")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult<int?>> CreateUserAsync([FromBody] UpsertOfficeUserCommand command)
    {
        var userId = await Mediator.Send(command);
        return Created("", new { UserId = userId });
    }

    /// <summary>
    /// Deletes a user globally including all office associations.
    /// </summary>
    /// <param name="userId">ID of the user to delete.</param>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("users/{userId}")]
    [HasPermission(Common.Application.Auth.Permissions.DeleteUser)]
    public async Task<ActionResult> DeleteUserAsync(int userId)
    {
        await Mediator.Send(new DeleteOfficeUserCommand(null, userId));
        return NoContent();
    }

    /// <summary>
    /// Deletes a user from a specific office. If it's the user's last office, also deletes from portal.
    /// </summary>
    /// <param name="userId">ID of the user to delete.</param>
    /// <param name="officeId">ID of the office to delete the user from.</param>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("users/{userId}/offices/{officeId}")]
    [HasPermission(Common.Application.Auth.Permissions.DeleteUser)]
    public async Task<ActionResult> DeleteUserAsync(int userId, int officeId)
    {
        await Mediator.Send(new DeleteOfficeUserCommand(officeId, userId));
        return NoContent();
    }

    /// <summary>
    /// Globally activates a user across all office associations.
    /// </summary>
    /// <param name="userId">The ID of the user to activate.</param>
    [HttpPut("users/{userId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> ActivateUserGloballyAsync(int userId)
    {
        await Mediator.Send(new ActivateUserCommand(null, userId, true));
        return NoContent();
    }

    /// <summary>
    /// Activates a user for a specific office.
    /// </summary>
    /// <param name="userId">The ID of the user to activate.</param>
    /// <param name="officeId">The ID of the office to activate the user for.</param>
    [HttpPut("users/{userId}/offices/{officeId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> ActivateUserForOfficeAsync(int userId, int officeId)
    {
        await Mediator.Send(new ActivateUserCommand(officeId, userId, true));
        return NoContent();
    }

    /// <summary>
    /// Globally deactivates a user across all office associations.
    /// </summary>
    /// <param name="userId">The ID of the user to deactivate.</param>
    [HttpPut("users/{userId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> DeactivateUserGloballyAsync(int userId)
    {
        await Mediator.Send(new ActivateUserCommand(null, userId, false));
        return NoContent();
    }

    /// <summary>
    /// Deactivates a user for a specific office.
    /// </summary>
    /// <param name="userId">The ID of the user to deactivate.</param>
    /// <param name="officeId">The ID of the office to deactivate the user from.</param>
    [HttpPut("users/{userId}/offices/{officeId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateUser)]
    public async Task<ActionResult> DeactivateUserForOfficeAsync(int userId, int officeId)
    {
        await Mediator.Send(new ActivateUserCommand(officeId, userId, false));
        return NoContent();
    }

    /// <summary>
    /// Activates a SuperAdmin user in SA Portal and syncs with Azure AD B2C.
    /// </summary>
    /// <param name="userId">The ID of the SuperAdmin user to activate.</param>
    [HttpPut("superadmin-users/{userId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateSuperAdminUsers)]
    public async Task<ActionResult> ActivateSuperAdminUserAsync(int userId)
    {
        await Mediator.Send(new ActivateSuperAdminUserCommand(userId, true));
        return NoContent();
    }

    /// <summary>
    /// Deactivates a SuperAdmin user in SA Portal and syncs with Azure AD B2C.
    /// </summary>
    /// <param name="userId">The ID of the SuperAdmin user to deactivate.</param>
    [HttpPut("superadmin-users/{userId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateSuperAdminUsers)]
    public async Task<ActionResult> DeactivateSuperAdminUserAsync(int userId)
    {
        await Mediator.Send(new ActivateSuperAdminUserCommand(userId, false));
        return NoContent();
    }
}
