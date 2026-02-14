using Microsoft.AspNetCore.Mvc;
using W2K.Common.Application.Controllers;
using W2K.Identity.Application.Commands;
using Microsoft.AspNetCore.Http;
using W2K.Identity.Application.Queries;
using W2K.Common.Application.Auth;
using W2K.Identity.Application.Enums;
using W2K.Identity.Application.DTOs;
using W2K.Common.Entities;
using W2K.Identity.Application;
using W2K.Common.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace W2K.Identity.Controllers.Offices;

[ApiController]
[Route("api/v{version:apiVersion}/offices")]
public class OfficesController : BaseApiController
{
    /// <summary>
    /// Creates a new office.
    /// </summary>
    /// <param name="command">The command containing office details.</param>
    /// <returns>The ID of the created office.</returns>
    /// <response code="201">Returns the ID of the created office</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult<int>> CreateOfficeAsync([FromBody] UpsertOfficeCommand command)
    {
        var officeId = await Mediator.Send(command);
        return Created("", new { id = officeId });
    }

    /// <summary>
    /// Updates an existing office.
    /// </summary>
    /// <param name="officeId">The ID of the office to update.</param>
    /// <param name="command">The command containing updated office details.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{officeId}")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> UpdateOfficeAsync(int officeId, [FromBody] UpsertOfficeCommand command)
    {
        command.SetOfficeId(officeId);
        _ = await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Upserts office owners.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing office owner details.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{officeId}/owners")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOfficeOwner)]
    public async Task<ActionResult> UpsertOfficeOwnersAsync(int officeId, [FromBody] UpsertOfficeOwnerCommand command)
    {
        command.SetOfficeId(officeId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets all office owners associated with the current user.
    /// </summary>
    /// <returns>A list of office owners for the current user.</returns>
    [HttpGet("owners")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOfficeOwner)]
    public async Task<ActionResult<List<OfficesOwnersListItemDto>>> GetOfficeOwnersListAsync()
    {
        var officeOwners = await Mediator.Send(new GetOfficeOwnersListQuery());
        return Ok(officeOwners);
    }

    /// <summary>
    /// Gets the details of a specific office owner by office and owner information.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="firstName">The first name of the office owner.</param>
    /// <param name="lastName">The last name of the office owner.</param>
    /// <param name="email">The email address of the office owner.</param>
    /// <param name="mobilePhone">The mobile phone number of the office owner.</param>
    /// <returns>The details of the specified office owner.</returns>
    [HttpGet("{officeId}/owners")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOfficeOwner)]
    public async Task<ActionResult<OfficeOwnerDto>> GetOfficeOwnerAsync(int officeId, string firstName, string lastName, string email, string mobilePhone)
    {
        var officeOwner = await Mediator.Send(new GetOfficeOwnerQuery(officeId, firstName, lastName, email, mobilePhone));
        return Ok(officeOwner);
    }

    /// <summary>
    /// Upserts office bank account.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="command">The command containing office bank account details.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{officeId}/bank-accounts")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOfficeBankAccount)]
    public async Task<ActionResult> UpsertOfficeBankAccountAsync(int officeId, [FromBody] UpsertOfficeBankAccountCommand command)
    {
        command.SetOfficeId(officeId);
        await Mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Gets the details of a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <returns>The details of the office with the given ID.</returns>
    [HttpGet("{officeId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(AuthPolicies.TokenOrSession)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<OfficeDto>> GetOfficeDetailsAsync(int officeId)
    {
        var officeDetails = await Mediator.Send(new GetOfficeDetailsQuery(officeId));
        return Ok(officeDetails);
    }

    /// <summary>
    /// Completes and submits office enrollment.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{officeId}/submit")]
    [HasPermission(Common.Application.Auth.Permissions.UpdateOffice)]
    public async Task<ActionResult> SubmitOfficeEnrollmentAsync(int officeId)
    {
        await Mediator.Send(new SubmitOfficeEnrollmentCommand(officeId));
        return NoContent();
    }

    /// <summary>
    /// Gets office enrollment details for the logged-in merchant admin.
    /// </summary>
    /// <returns>Office enrollment details.</returns>
    /// <response code="200">Returns the office enrollment details</response>
    [HttpGet("{officeId}/enrollment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<OfficeEnrollmentDetailDto>> GetOfficeEnrollmentDetailAsync(int officeId)
    {
        var result = await Mediator.Send(new GetOfficeEnrollmentDetailQuery(officeId));
        return Ok(result);
    }

    /// <summary>
    /// Get merchant offices for the logged-in user with filtering and sorting.
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="search">Search term for Name or Id of Office (optional).</param>
    /// <param name="sortBy">Sort column (optional).</param>
    /// <param name="sortDescending">Sort descending (optional).</param>
    /// <returns>List of merchant offices for the logged-in user.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<PagedList<OfficeListItemDto>>> GetMerchantOfficesAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        OfficeSortColumn sortBy = OfficeSortColumn.LastUpdated,
        bool sortDescending = true)
    {
        var result = await Mediator.Send(new GetUserOfficesQuery(page, pageSize, search, sortBy, sortDescending));
        return Ok(result);
    }

    /// <summary>
    /// Searches for offices accessible to the current user.
    /// </summary>
    /// <param name="searchTerm">The search term to filter offices.</param>
    /// <returns>Returns a list of matching offices accessible to the user.</returns>
    /// <response code="200">Returns the list of matching offices.</response>
    /// <remarks>
    /// Note: This endpoint is intentionally duplicated in SuperAdminOfficesController with a different route
    /// (/api/v{version}/admin/offices/search) to maintain separate authorization contexts and REST conventions.
    /// </remarks>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices)]
    public async Task<ActionResult<IList<OfficeSearchResultDto>>> SearchOfficesAsync([FromQuery] string searchTerm)
    {
        var result = await Mediator.Send(new SearchOfficesQuery(searchTerm));
        return Ok(result);
    }
}
