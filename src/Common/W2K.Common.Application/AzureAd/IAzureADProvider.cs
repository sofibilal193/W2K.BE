namespace DFI.Common.Application.AzureAd;

/// <summary>
/// Defines operations for managing Azure AD B2C users via Microsoft Graph.
/// Implementations should encapsulate Graph calls and map results to domain models.
/// </summary>
public interface IAzureADProvider : IDisposable
{
    /// <summary>
    /// Checks whether a user already exists in Azure AD B2C for the given email address.
    /// The check follows the same identity model used by <see cref="AddUserAsync"/>,
    /// i.e., a local account identity with sign-in type "emailAddress" and issuer equal to the configured B2C domain.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>True if a matching user exists; otherwise false.</returns>
    Task<bool> UserExistsByEmailAsync(
        string email,
        CancellationToken cancel = default);

    /// <summary>
    /// Retrieves an Azure AD user by object identifier.
    /// </summary>
    /// <param name="id">The Azure AD object id of the user.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>The matching user.</returns>
    Task<AzureADUser> GetUserAsync(
        string id,
        CancellationToken cancel = default);

    /// <summary>
    /// Retrieves an Azure AD user's sign-in info by object identifier.
    /// </summary>
    /// <param name="id">The Azure AD object id of the user.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>The matching user sign-in info.</returns>
    Task<AzureADUserSignInInfo> GetUserSignInInfoAsync(
        string id,
        CancellationToken cancel = default);

    /// <summary>
    /// Updates an existing Azure AD user with the provided values.
    /// Only supported fields will be applied by the implementation.
    /// </summary>
    /// <param name="user">The user values to update.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>A status indicating the outcome.</returns>
    Task<AzureAdResponseStatus> UpdateUserAsync(
        AzureADUser user,
        CancellationToken cancel = default);

    /// <summary>
    /// Deletes the specified Azure AD user.
    /// </summary>
    /// <param name="id">The Azure AD object id of the user.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// <see cref="AzureAdResponseStatus.Success"/> if the user was deleted,
    /// <see cref="AzureAdResponseStatus.Notfound"/> if the user does not exist,
    /// otherwise <see cref="AzureAdResponseStatus.Failed"/>.
    /// </returns>
    Task<AzureAdResponseStatus> DeleteUserAsync(
        string id,
        CancellationToken cancel = default);

    /// <summary>
    /// Resets multi-factor authentication for the user and optionally sets a new mobile phone.
    /// Implementations may remove existing phone methods and re-add the supplied number.
    /// </summary>
    /// <param name="id">The Azure AD object id of the user.</param>
    /// <param name="mobilePhone">An optional E.164 formatted mobile phone to set (e.g., +11234567890).</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>A status indicating the outcome.</returns>
    Task<AzureAdResponseStatus> Reset2faAsync(
        string id,
        string? mobilePhone,
        CancellationToken cancel = default);

    /// <summary>
    /// Enables or disables a user account. Implementations may revoke active sessions when disabling.
    /// </summary>
    /// <param name="id">The Azure AD object id of the user.</param>
    /// <param name="isEnabled">True to enable; false to disable.</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>A status indicating the outcome.</returns>
    Task<AzureAdResponseStatus> ToggleUserStatusAsync(
        string id,
        bool isEnabled,
        CancellationToken cancel = default);

    /// <summary>
    /// Creates a new Azure AD B2C local account (email sign-in) and returns the created user
    /// along with a generated strong temporary password. The password is marked to require
    /// change on the next sign-in.
    /// </summary>
    /// <param name="user">The user details to create (FirstName, LastName, Email, optional MobilePhone and JobTitle).</param>
    /// <param name="cancel">A token to observe while waiting for the task to complete.</param>
    /// <returns>Result containing the created user, generated temporary password, and response status.</returns>
    Task<AddUserResponse> AddUserAsync(
        AzureADUser user,
        CancellationToken cancel = default);
}

/// <summary>
/// Standard result codes for Azure AD provider operations.
/// </summary>
public enum AzureAdResponseStatus
{
    /// <summary>Operation completed successfully.</summary>
    Success = 0,
    /// <summary>Operation failed due to an unexpected error.</summary>
    Failed = 1,
    /// <summary>Resource already exists (for example, attempting to create a duplicate user).</summary>
    Duplicate = 2,
    /// <summary>Resource was not found.</summary>
    Notfound = 3
}

/// <summary>
/// The response from a successful or attempted user creation operation.
/// </summary>
/// <param name="User">The created Azure AD user.</param>
/// <param name="TemporaryPassword">The generated strong temporary password (requires change on next sign-in).</param>
/// <param name="Status">The operation status.</param>
public readonly record struct AddUserResponse(AzureADUser User, string TemporaryPassword, AzureAdResponseStatus Status);
