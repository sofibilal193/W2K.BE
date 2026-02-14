using W2K.Common.Application.DTOs;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the Identity service.
/// </summary>
public interface IIdentityApiClient
{
    /// <summary>
    /// Gets an office by its ID.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>The office details or null if not found.</returns>
    Task<OfficeDto> GetOfficeDetailsAsync(int officeId, CancellationToken cancel = default);

    /// <summary>
    /// Gets all lenders.
    /// </summary>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>A list of lenders.</returns>
    Task<List<LenderDto>> GetLendersAsync(CancellationToken cancel = default);

    /// <summary>
    /// Gets permissions for a user and office.
    /// </summary>
    /// <param name="userId">The ID of the user (optional).</param>
    /// <param name="officeId">The ID of the office (optional).</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>A list of permissions.</returns>
    Task<IList<string>> GetUserPermissionsAsync(int? userId, int? officeId, CancellationToken cancel = default);

    /// <summary>
    /// Checks if a user has access to a specific office.
    /// </summary>
    /// <param name="officeId">The ID of the office (optional).</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>True if the user has access, otherwise false.</returns>
    Task<bool> ValidateOfficeUserAsync(int officeId, CancellationToken cancel = default);
}
