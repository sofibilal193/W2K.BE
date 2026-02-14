using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace DFI.Common.Infrastructure.AzureAd;

/// <summary>
/// Default implementation of <see cref="IGraphService"/> backed by <see cref="GraphServiceClient"/>.
/// This class forwards calls to Microsoft Graph and does not add custom logic.
/// </summary>
public class GraphService(GraphServiceClient client) : IGraphService
{
    private readonly GraphServiceClient _client = client;

    /// <inheritdoc />
    public async Task<UserCollectionResponse?> FindUsersByEmailIdentityAsync(string email, string? issuerDomain, CancellationToken cancel = default)
    {
        // Build filter for B2C local account identity using identities collection
        // identities/any(i: i/signInType eq 'emailAddress' and i/issuerAssignedId eq '{email}' [and i/issuer eq '{domain}'])
        var escapedEmail = email.Replace("'", "''", StringComparison.Ordinal);
        var filter = $"identities/any(i: i/issuerAssignedId eq '{escapedEmail}'";
        if (!string.IsNullOrWhiteSpace(issuerDomain))
        {
            var escapedDomain = issuerDomain.Replace("'", "''", StringComparison.Ordinal);
            filter += $" and i/issuer eq '{escapedDomain}'";
        }
        filter += ")";

        var result = await _client.Users.GetAsync(
            requestConfiguration: x =>
            {
                x.QueryParameters.Filter = filter;
                x.QueryParameters.Top = 1; // only need to know if any exists
                x.QueryParameters.Select = ["id"];
            },
            cancellationToken: cancel);

        return result;
    }

    /// <inheritdoc />
    public async Task<User?> GetUserAsync(string id, CancellationToken cancel = default)
    {
        return await _client.Users[id].GetAsync(cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task<User?> CreateUserAsync(User user, CancellationToken cancel = default)
    {
        return await _client.Users.PostAsync(user, cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task<PhoneAuthenticationMethodCollectionResponse?> GetPhoneMethodsAsync(string id, CancellationToken cancel = default)
    {
        return await _client.Users[id].Authentication.PhoneMethods.GetAsync(cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task PatchUserAsync(string id, User user, CancellationToken cancel = default)
    {
        _ = await _client.Users[id].PatchAsync(user, cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task PatchPhoneMethodAsync(string userId, string methodId, PhoneAuthenticationMethod method, CancellationToken cancel = default)
    {
        _ = await _client.Users[userId].Authentication.PhoneMethods[methodId].PatchAsync(method, cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task AddPhoneMethodAsync(string userId, PhoneAuthenticationMethod method, CancellationToken cancel = default)
    {
        _ = await _client.Users[userId].Authentication.PhoneMethods.PostAsync(method, cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task DeleteUserAsync(string id, CancellationToken cancel = default)
    {
        await _client.Users[id].DeleteAsync(cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task DeletePhoneMethodAsync(string userId, string methodId, CancellationToken cancel = default)
    {
        await _client.Users[userId].Authentication.PhoneMethods[methodId].DeleteAsync(cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task RevokeSignInSessionsAsync(string id, CancellationToken cancel = default)
    {
        _ = await _client.Users[id].RevokeSignInSessions.PostAsRevokeSignInSessionsPostResponseAsync(cancellationToken: cancel);
    }

    /// <inheritdoc />
    public async Task<SignIn?> GetLastSignInInfoAsync(string id, CancellationToken cancel = default)
    {

        var result = await _client.AuditLogs.SignIns.GetAsync(
            requestConfiguration: x =>
            {
                x.QueryParameters.Filter = $"userId eq '{id}' and status/errorCode eq 0";
                x.QueryParameters.Orderby = ["createdDateTime desc"];
                x.QueryParameters.Select = ["createdDateTime", "ipAddress"];
                x.QueryParameters.Top = 1;
            },
            cancellationToken: cancel);

        return result?.Value?.FirstOrDefault();
    }
}
