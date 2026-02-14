using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using W2K.Identity.Application.Enums;
using Microsoft.AspNetCore.Http;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application;
using W2K.Common.Application.DTOs;

namespace W2K.Identity.Controllers.SuperAdmin;

public partial class SuperAdminController : BaseSuperAdminApiController
{
    /// <summary>
    /// Retrieves a paginated list of offices for Super Admins.
    /// </summary>
    /// <param name="page">The page number for pagination.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="search">Optional search term to filter offices by Id or Name.</param>
    /// <param name="sortBy">The field to sort by (e.g., name, lastLogin).</param>
    /// <param name="sortDescending">Whether to sort in descending order.</param>
    /// <returns>A paginated list of office records.</returns>
    /// <response code="200">Returns the paginated list of offices.</response>
    [HttpGet("offices")]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<PagedList<OfficeListItemDto>>> GetOfficesAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        OfficeSortColumn sortBy = OfficeSortColumn.LastUpdated,
        bool sortDescending = true)
    {
        var query = new GetOfficesForSuperAdminQuery(page, pageSize, search, sortBy, sortDescending);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Updates office owner information.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing office owner details.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("offices/{officeId}/owners")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOfficeOwner)]
    public async Task<ActionResult> UpsertOfficeOwnersAsync(int officeId, UpsertOfficeOwnerCommand command)
    {
        command.SetOfficeId(officeId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Updates office information
    /// </summary>
    /// <param name="officeId">The ID of the office to update.</param>
    /// <param name="command">The command containing updated office details.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("offices/{officeId}")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> UpdateOfficeAsync(int officeId, [FromBody] UpsertOfficeCommand command)
    {
        command.SetOfficeId(officeId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Retrieves owners for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office to retrieve owners for.</param>
    /// <returns>Returns the list of office owners.</returns>
    /// <response code="200">Returns the list of office owners.</response>
    [HttpGet("offices/{officeId}/owners")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOfficeOwner)]
    public async Task<ActionResult<List<OfficeOwnerDto>>> GetOfficeOwnersAsync(int officeId)
    {
        var result = await Mediator.Send(new GetOfficeOwnersQuery(officeId));
        return Ok(result);
    }

    /// <summary>
    /// Retrieves bank account information for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <returns>Returns the office bank account information.</returns>
    /// <response code="200">Returns the office bank account information.</response>
    [HttpGet("offices/{officeId}/bankAccount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOfficeBankAccount)]
    public async Task<ActionResult<OfficeBankAccountDto>> GetOfficeBankAccountAsync(int officeId)
    {
        var result = await Mediator.Send(new GetOfficeBankAccountsQuery(officeId));
        return Ok(result);
    }

    /// <summary>
    /// Activates an office.
    /// </summary>
    /// <param name="officeId">The ID of the office to update.</param>
    [HttpPut("offices/{officeId}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> ActivateOfficeAsync(int officeId)
    {
        await Mediator.Send(new ActivateOfficeCommand(officeId, true));
        return NoContent();
    }

    /// <summary>
    /// Deactivates an office.
    /// </summary>
    /// <param name="officeId">The ID of the office to update.</param>
    [HttpPut("offices/{officeId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> DeactivateOfficeAsync(int officeId)
    {
        await Mediator.Send(new ActivateOfficeCommand(officeId, false));
        return NoContent();
    }

    /// <summary>
    /// Updates bank account information for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing bank account details.</param>
    /// <returns>Returns no content on successful update.</returns>
    /// <response code="204">Bank account information updated successfully.</response>
    [HttpPut("offices/{officeId}/bankAccount")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOfficeBankAccount)]
    public async Task<ActionResult> UpsertOfficeBankAccountAsync(int officeId, UpsertOfficeBankAccountCommand command)
    {
        command.SetOfficeId(officeId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Retrieves detailed information for a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the Office.</param>
    /// <returns>Returns the office details.</returns>
    /// <response code="200">Returns the office details.</response>
    [HttpGet("offices/{officeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<OfficeInfoDto>> GetOfficeInfoAsync(int officeId)
    {
        var result = await Mediator.Send(new GetOfficeInfoQuery(officeId));
        return Ok(result);
    }

    /// <summary>
    /// Updates the statuses of an office.
    /// </summary>
    /// <param name="officeId">The ID of the office to update.</param>
    /// <param name="command">The command containing status information.</param>
    /// <response code="204">Status update was successful.</response>
    [HttpPut("offices/{officeId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> UpdateOfficeStatusAsync(int officeId, [FromBody] UpdateOfficeStatusCommand command)
    {
        command.SetOfficeId(officeId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Searches for offices by name or identifier.
    /// </summary>
    /// <param name="searchTerm">The search term to filter offices.</param>
    /// <returns>Returns a list of matching offices.</returns>
    /// <response code="200">Returns the list of matching offices.</response>
    [HttpGet("offices/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<IList<OfficeSearchResultDto>>> SearchOfficesAsync([FromQuery] string searchTerm)
    {
        var result = await Mediator.Send(new SearchOfficesQuery(searchTerm));
        return Ok(result);
    }
}
