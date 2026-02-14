#pragma warning disable CA2213 // Disposable fields should be disposed
using System.Net;
using W2K.Common.Application.AzureAd;
using W2K.Common.Utils;
using W2K.Common.Crypto;
using W2K.Common.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace W2K.Common.Infrastructure.AzureAd;

/// <summary>
/// Azure AD provider that implements user management operations using Microsoft Graph.
/// This class maps Graph models to application models and handles common error mapping and logging.
/// </summary>
public class AzureADProvider(
    ILogger<AzureADProvider> logger,
    IGraphService service,
    ICryptoProvider crypto,
    AzureADSettings? settings = null) : IAzureADProvider
{
    private readonly IGraphService _service = service;
    private readonly ILogger<AzureADProvider> _logger = logger;
    private readonly ICryptoProvider _crypto = crypto;
    private readonly AzureADSettings _settings = settings ?? new AzureADSettings();

    #region IAzureADProvider Methods

    /// <inheritdoc />
    public async Task<bool> UserExistsByEmailAsync(string email, CancellationToken cancel = default)
    {
        try
        {
            var normalized = email?.Trim().ToLowerInvariant() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            var result = await _service.FindUsersByEmailIdentityAsync(normalized, _settings.Domain, cancel);
            return (result?.Value?.Count ?? 0) > 0;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error checking user existence by email {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence by email {Email}", email);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<AzureADUser> GetUserAsync(string id, CancellationToken cancel = default)
    {
        var user = await _service.GetUserAsync(id, cancel);
        string? mobilePhone = null;
        if (user is not null)
        {
            // Query phone authentication methods for the user
            var phoneMethods = await _service.GetPhoneMethodsAsync(id, cancel);
            if (phoneMethods?.Value is not null
                && phoneMethods.Value.FirstOrDefault(x => x is PhoneAuthenticationMethod phone && phone.PhoneType == AuthenticationPhoneType.Mobile) is PhoneAuthenticationMethod mobileMethod)
            {
                mobilePhone = mobileMethod.PhoneNumber.FromB2CPhoneNumber();
            }

            return new AzureADUser(
                user.Id ?? string.Empty,
                user.GivenName ?? string.Empty,
                user.Surname ?? string.Empty,
                user.DisplayName ?? string.Empty,
                user.Mail ?? string.Empty,
                mobilePhone ?? user.MobilePhone ?? string.Empty,
                user.JobTitle,
                null,
                null);
        }
        return default;
    }

    /// <inheritdoc />
    public async Task<AzureADUserSignInInfo> GetUserSignInInfoAsync(string id, CancellationToken cancel = default)
    {
        // Get last sign-in info with timeout protection
        // Azure AD audit logs can be slow, so we add a timeout and error handling
        DateTimeOffset? lastSignInDate = null;
        string? lastSignInIp = null;

        try
        {
            var lastSignIn = await _service.GetLastSignInInfoAsync(id, cancel);
            lastSignInDate = lastSignIn?.CreatedDateTime;
            lastSignInIp = lastSignIn?.IpAddress;
        }
        catch (OperationCanceledException ex) when (cancel.IsCancellationRequested)
        {
            // Original cancellation token was cancelled - log and continue without sign-in info
            _logger.LogWarning(ex, "Request cancelled while retrieving last sign-in info for user {Id}, continuing without sign-in data", id);
        }
        catch (TaskCanceledException ex)
        {
            // HTTP request timeout - log and continue without sign-in info
            _logger.LogWarning(ex, "HTTP timeout retrieving last sign-in info for user {Id}, continuing without sign-in data", id);
        }
        catch (OperationCanceledException ex)
        {
            // Timeout occurred - log and continue without sign-in info
            _logger.LogWarning(ex, "Timeout retrieving last sign-in info for user {Id}, continuing without sign-in data", id);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire operation
            _logger.LogWarning(ex, "Failed to retrieve last sign-in info for user {Id}, continuing without sign-in data", id);
        }

        return new AzureADUserSignInInfo(lastSignInDate, lastSignInIp);
    }

    /// <inheritdoc />
    public async Task<AzureAdResponseStatus> UpdateUserAsync(AzureADUser user, CancellationToken cancel = default)
    {
        try
        {
            var patchUser = new User
            {
                GivenName = user.FirstName,
                Surname = user.LastName,
                DisplayName = user.FullName,
                Mail = user.Email.ToLowerInvariant(),
                MobilePhone = user.MobilePhone.ToB2CPhoneNumber(),
                JobTitle = user.JobTitle
            };
            await _service.PatchUserAsync(user.Id, patchUser, cancel);

            // Update mobile phone authentication method if MobilePhone is specified
            if (!string.IsNullOrEmpty(user.MobilePhone))
            {
                var phoneMethods = await _service.GetPhoneMethodsAsync(user.Id, cancel);
                var mobileMethod = phoneMethods?.Value?.FirstOrDefault(x => x is PhoneAuthenticationMethod phone && phone.PhoneType == AuthenticationPhoneType.Mobile);
                if (string.IsNullOrEmpty(mobileMethod?.Id))
                {
                    // Add new mobile phone method
                    await _service.AddPhoneMethodAsync(
                        user.Id,
                        new PhoneAuthenticationMethod
                        {
                            PhoneNumber = user.MobilePhone.ToB2CPhoneNumber(),
                            PhoneType = AuthenticationPhoneType.Mobile
                        },
                        cancel);
                }
                else
                {
                    // Update existing mobile phone method
                    await _service.PatchPhoneMethodAsync(
                        user.Id,
                        mobileMethod.Id,
                        new PhoneAuthenticationMethod
                        {
                            PhoneNumber = user.MobilePhone.ToB2CPhoneNumber(),
                            PhoneType = AuthenticationPhoneType.Mobile
                        },
                        cancel);
                }
            }

            return AzureAdResponseStatus.Success;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error updating user {Id}", user.Id);
            return ex.ResponseStatusCode switch
            {
                (int)HttpStatusCode.NotFound => AzureAdResponseStatus.Notfound,
                _ when ex.Error?.Message?.Contains("A conflicting object", StringComparison.OrdinalIgnoreCase) == true =>
                    AzureAdResponseStatus.Duplicate,
                _ => AzureAdResponseStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {Id}", user.Id);
            return AzureAdResponseStatus.Failed;
        }
    }

    /// <inheritdoc />
    public async Task<AzureAdResponseStatus> DeleteUserAsync(string id, CancellationToken cancel = default)
    {
        try
        {
            await _service.DeleteUserAsync(id, cancel);
            return AzureAdResponseStatus.Success;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error deleting user {Id}", id);
            return ex.ResponseStatusCode switch
            {
                (int)HttpStatusCode.NotFound => AzureAdResponseStatus.Notfound,
                _ => AzureAdResponseStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return AzureAdResponseStatus.Failed;
        }
    }

    /// <inheritdoc />
    public async Task<AzureAdResponseStatus> Reset2faAsync(string id, string? mobilePhone, CancellationToken cancel = default)
    {
        try
        {
            // Remove all phone authentication methods for the user
            var methods = await _service.GetPhoneMethodsAsync(id, cancel);
            if (methods?.Value is not null)
            {
                foreach (var methodId in methods.Value.Where(x => !string.IsNullOrEmpty(x.Id)).Select(x => x.Id))
                {
                    await _service.DeletePhoneMethodAsync(id, methodId!, cancel);
                }
            }
            // Optionally, add the new phone number as a method
            if (!string.IsNullOrEmpty(mobilePhone))
            {
                await _service.AddPhoneMethodAsync(
                    id,
                    new PhoneAuthenticationMethod
                    {
                        PhoneNumber = mobilePhone.ToB2CPhoneNumber(),
                        PhoneType = AuthenticationPhoneType.Mobile
                    },
                    cancel);
            }
            return AzureAdResponseStatus.Success;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error resetting 2FA for user {Id}", id);
            return ex.ResponseStatusCode switch
            {
                (int)HttpStatusCode.NotFound => AzureAdResponseStatus.Notfound,
                _ => AzureAdResponseStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting 2FA for user {Id}", id);
            return AzureAdResponseStatus.Failed;
        }
    }

    /// <inheritdoc />
    public async Task<AzureAdResponseStatus> ToggleUserStatusAsync(string id, bool isEnabled, CancellationToken cancel = default)
    {
        try
        {
            await _service.PatchUserAsync(
                id,
                new User
                {
                    AccountEnabled = isEnabled
                },
                cancel);

            // If disabling the user, revoke all sessions
            if (!isEnabled)
            {
                try
                {
                    await _service.RevokeSignInSessionsAsync(id, cancel);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to revoke sessions for user {Id} after disabling.", id);
                }
            }
            return AzureAdResponseStatus.Success;
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error toggling user status {Id}", id);
            return ex.ResponseStatusCode switch
            {
                (int)HttpStatusCode.NotFound => AzureAdResponseStatus.Notfound,
                _ => AzureAdResponseStatus.Failed
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status {Id}", id);
            return AzureAdResponseStatus.Failed;
        }
    }

    /// <inheritdoc />
    public async Task<AddUserResponse> AddUserAsync(AzureADUser user, CancellationToken cancel = default)
    {
        try
        {
            // Generate a strong temporary password
            var password = _crypto.GenerateRandomPassword(10, strong: true);

            var newUser = new User
            {
                GivenName = user.FirstName,
                Surname = user.LastName,
                DisplayName = string.Concat(user.FirstName, " ", user.LastName).Trim(),
                Mail = user.Email.ToLowerInvariant(),
                MobilePhone = user.MobilePhone.ToB2CPhoneNumber(),
                JobTitle = user.JobTitle,
                AccountEnabled = true,
                PasswordProfile = new PasswordProfile
                {
                    ForceChangePasswordNextSignIn = true,
                    Password = password
                },
                // For B2C local accounts, set identities instead of UPN
                Identities =
                [
                    new ObjectIdentity
                    {
                        SignInType = "emailAddress",
                        Issuer = string.IsNullOrWhiteSpace(_settings.Domain) ? null : _settings.Domain,
                        IssuerAssignedId = user.Email.ToLowerInvariant()
                    }
                ],
                MailNickname = user.Email.Split('@')[0]
            };

            // Create user
            var created = await _service.CreateUserAsync(newUser, cancel);

            if (created is null || string.IsNullOrEmpty(created.Id))
            {
                return new AddUserResponse(default, string.Empty, AzureAdResponseStatus.Failed);
            }

            // Add phone method if provided
            if (!string.IsNullOrEmpty(user.MobilePhone))
            {
                try
                {
                    await _service.AddPhoneMethodAsync(
                        created.Id!,
                        new PhoneAuthenticationMethod
                        {
                            PhoneNumber = user.MobilePhone.ToB2CPhoneNumber(),
                            PhoneType = AuthenticationPhoneType.Mobile
                        },
                        cancel);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to add phone method for user {Id}", created.Id);
                }
            }

            // Map back
            var resultUser = new AzureADUser(
                created.Id!,
                created.GivenName ?? user.FirstName,
                created.Surname ?? user.LastName,
                created.DisplayName ?? user.FullName,
                created.Mail ?? user.Email.ToLowerInvariant(),
                user.MobilePhone,
                created.JobTitle,
                null,
                null);

            return new AddUserResponse(resultUser, password, AzureAdResponseStatus.Success);
        }
        catch (ODataError ex)
        {
            _logger.LogError(ex, "OData error creating user {Email}", user.Email);
            var status = ex.ResponseStatusCode switch
            {
                (int)HttpStatusCode.Conflict => AzureAdResponseStatus.Duplicate,
                _ => AzureAdResponseStatus.Failed
            };
            return new AddUserResponse(default, string.Empty, status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Email}", user.Email);
            return new AddUserResponse(default, string.Empty, AzureAdResponseStatus.Failed);
        }
    }

    #endregion

    #region IDisposable Methods

    /// <summary>Disposes this instance.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose to release managed/unmanaged resources. No-op currently.
    /// </summary>
    /// <param name="disposing">True when called from <see cref="Dispose()"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }

    #endregion
}
#pragma warning restore CA2213 // Disposable fields should be disposed
