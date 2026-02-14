using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Enums;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.Users;

[Route("api/v{version:apiVersion}/users")]
[ApiController]
public class UsersController(IMediator mediator) : BaseApiController
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get users for the offices of the logged-in user.
    /// </summary>
    /// <returns>List of users for the offices of the logged-in user.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedList<OfficeUserDto>>> GetOfficeUsersForLoggedInUserAsync(
        int page = 1,
        int pageSize = 10,
        OfficeUsersSortColumn sortBy = OfficeUsersSortColumn.LastUpdated,
        bool sortDescending = true)
    {
        var result = await _mediator.Send(new GetOfficeUsersQuery(page, pageSize, null, sortBy, sortDescending));
        return Ok(result);
    }

    /// <summary>
    /// Get offices of the logged-in user.
    /// </summary>
    /// <returns>List of offices for the logged-in user.</returns>

    [HttpGet("offices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<UserOfficeListItemDto>>> GetOfficeListForLoggedInUserAsync()
    {
        var result = await _mediator.Send(new GetUserOfficeListQuery());
        return Ok(result);
    }

    /// <summary>
    /// Gets user invitation information (anonymous endpoint).
    /// </summary>
    /// <param name="userId">ID of User (route parameter).</param>
    /// <param name="officeId">ID of Office (optional query parameter).</param>
    /// <returns>User invitation information.</returns>
    [HttpGet("{userId:int}/invitation")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserInviteInfoDto>> GetUserInvitationInfoAsync(int userId, [FromQuery] int? officeId = null)
    {
        var result = await _mediator.Send(new GetUserInviteInfoQuery(userId, officeId));
        return Ok(result);
    }

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
        var result = await _mediator.Send(new SearchUsersQuery(email));
        return Ok(result);
    }
}
