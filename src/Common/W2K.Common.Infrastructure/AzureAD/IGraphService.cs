using Microsoft.Graph.Models;

namespace DFI.Common.Infrastructure.AzureAd;

/// <summary>
/// Thin abstraction over Microsoft Graph operations used by the Azure AD provider.
/// Implementations should delegate to a configured GraphServiceClient instance.
/// </summary>
public interface IGraphService
{
    /// <summary>
    /// Finds users matching the specified email based on B2C local account identity
    /// (identities/any(i: i/signInType eq 'emailAddress' and i/issuerAssignedId eq '{email}' and i/issuer eq '{domain}')).
    /// </summary>
    /// <param name="email">The email address to match (case-insensitive).</param>
    /// <param name="issuerDomain">The B2C domain used as identity issuer; if null or empty, issuer clause is omitted.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    /// <returns>A collection of users that match the email identity.</returns>
    Task<UserCollectionResponse?> FindUsersByEmailIdentityAsync(string email, string? issuerDomain, CancellationToken cancel = default);

    /// <summary>
    /// Retrieves a user by Azure AD object identifier.
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    /// <returns>The user, or null if not found.</returns>
    Task<User?> GetUserAsync(string id, CancellationToken cancel = default);

    /// <summary>
    /// Creates a new user in Azure AD/B2C.
    /// </summary>
    /// <param name="user">The user payload to create.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    /// <returns>The created user, or null if creation failed.</returns>
    Task<User?> CreateUserAsync(User user, CancellationToken cancel = default);

    /// <summary>
    /// Lists phone authentication methods configured for a user.
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    /// <returns>A collection of phone methods, or null if none found.</returns>
    Task<PhoneAuthenticationMethodCollectionResponse?> GetPhoneMethodsAsync(string id, CancellationToken cancel = default);

    /// <summary>
    /// Applies a partial update to the specified user.
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="user">The patch payload.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task PatchUserAsync(string id, User user, CancellationToken cancel = default);

    /// <summary>
    /// Applies a partial update to an existing phone authentication method.
    /// </summary>
    /// <param name="userId">The user's object id.</param>
    /// <param name="methodId">The phone method id.</param>
    /// <param name="method">The patch payload.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task PatchPhoneMethodAsync(string userId, string methodId, PhoneAuthenticationMethod method, CancellationToken cancel = default);

    /// <summary>
    /// Adds a new phone authentication method to the specified user.
    /// </summary>
    /// <param name="userId">The user's object id.</param>
    /// <param name="method">The phone method to add.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task AddPhoneMethodAsync(string userId, PhoneAuthenticationMethod method, CancellationToken cancel = default);

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task DeleteUserAsync(string id, CancellationToken cancel = default);

    /// <summary>
    /// Deletes a specific phone authentication method from a user.
    /// </summary>
    /// <param name="userId">The user's object id.</param>
    /// <param name="methodId">The phone method id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task DeletePhoneMethodAsync(string userId, string methodId, CancellationToken cancel = default);

    /// <summary>
    /// Revokes all sign-in sessions for the user (forces re-authentication).
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    Task RevokeSignInSessionsAsync(string id, CancellationToken cancel = default);

    /// <summary>
    /// Retrieves the most recent sign-in event for a user, if available.
    /// </summary>
    /// <param name="id">The user's object id.</param>
    /// <param name="cancel">A token to cancel the request.</param>
    /// <returns>The latest sign-in record, or null if none exist.</returns>
    Task<SignIn?> GetLastSignInInfoAsync(string id, CancellationToken cancel = default);
}
