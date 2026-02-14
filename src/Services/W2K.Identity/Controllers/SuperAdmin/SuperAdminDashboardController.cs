using W2K.Common.Application.Auth;
using W2K.Common.Application.Controllers;
using W2K.Common.Application.Dtos;
using W2K.Common.Application.Queries.GlobalSearch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace W2K.Identity.Controllers.SuperAdmin;

public partial class SuperAdminController : BaseSuperAdminApiController
{
    /// <summary>
    /// Performs a global search across offices and users.
    /// </summary>
    /// <param name="searchTerm">The search term to find offices and users. Supports partial name matches or exact ID matches.</param>
    /// <returns>A list of search results containing offices and users.</returns>
    /// <response code="200">Returns the list of matching search results.</response>
    [HttpGet("dashboard/users-offices/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HasPermission(Common.Application.Auth.Permissions.ViewOffices, Common.Application.Auth.Permissions.ViewUsers)]
    public IAsyncEnumerable<GlobalSearchResultDto> GlobalSearchAsync([FromQuery] string searchTerm)
    {
        return Mediator.CreateStream(new GlobalSearchQuery(searchTerm));
    }
}
