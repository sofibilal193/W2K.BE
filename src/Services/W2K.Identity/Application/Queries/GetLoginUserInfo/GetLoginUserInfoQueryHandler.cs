using AutoMapper;
using W2K.Common.Application.AzureAd;
using W2K.Common.Enums;
using W2K.Common.Identity;
using W2K.Common.Utils;
using W2K.Identity.Application.Commands;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetLoginUserInfoQueryHandler(
    IIdentityUnitOfWork data,
    IMapper mapper,
    IMediator mediator,
    ILogger<GetLoginUserInfoQueryHandler> logger,
    ICurrentUser currentUser,
    IAzureADProvider azureADProvider,
    IServiceScopeFactory scopeFactory) : IRequestHandler<GetLoginUserInfoQuery, LoginUserInfoDto>
{
    private static readonly TimeSpan SignInInfoTimeout = TimeSpan.FromSeconds(60);
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<GetLoginUserInfoQueryHandler> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IAzureADProvider _azureADProvider = azureADProvider;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task<LoginUserInfoDto> Handle(GetLoginUserInfoQuery query, CancellationToken cancellationToken)
    {
        var mobilePhone = query.MobilePhone?.FromB2CPhoneNumber();
        var azureADUser = await GetAzureADUserAsync(query, cancellationToken);
        mobilePhone ??= azureADUser?.MobilePhone;

        var user = await FindOrCreateUserAsync(query, mobilePhone, azureADUser?.LastLoginIpAddress ?? _currentUser.Source, cancellationToken);
        if (user is not null)
        {
            await ProcessUserActionsAsync(user, query, mobilePhone, azureADUser?.LastLoginIpAddress ?? _currentUser.Source, cancellationToken);
            QueueLastLoginSourceRefresh(user.Id, query.ProviderId, query.IsTokenValidated, query.IsSignIn || query.IsSignUp);
        }
        return _mapper.Map<LoginUserInfoDto>(user);
    }

    private async Task<User?> FindOrCreateUserAsync(
        GetLoginUserInfoQuery query,
        string? mobilePhone,
        string? lastLoginIpAddress,
        CancellationToken cancel)
    {
        var user = await _data.Users
            .Include("Offices.Office")
            .Include("Offices.Role.Permissions")
            .FirstOrDefaultAsync(x => x.ProviderId == query.ProviderId, cancel);

        if (user is null)
        {
            _logger.LogInformation("No user found for ProviderId: {Id}.", query.ProviderId);
            user = await _data.Users
                .Include("Offices.Office")
                .Include("Offices.Role.Permissions")
                .FirstOrDefaultAsync(x => x.Email == query.Email, cancel);

            if (user is null)
            {
                _logger.LogInformation("No User found for {Email}.", query.Email);
                if (query.IsSignUp)
                {
                    _logger.LogInformation("New user signup. Email: {Email}", query.Email);
                    user = new User(
                        providerId: query.ProviderId,
                        firstName: query.FirstName,
                        lastName: query.LastName,
                        email: query.Email,
                        mobilePhone: mobilePhone,
                        lastLoginIpAddress: lastLoginIpAddress);
                    _data.Users.Add(user);
                }
            }
        }

        // Get all active (not disabled) office-user relationships for this user
        var officeUsers = user?.Offices.Where(x => !x.IsDisabled);
        // If the user is associated with any active offices, log their login event for each office
        if (officeUsers?.Any() == true)
        {
            foreach (var officeUser in officeUsers)
            {
                // Fetch the office entity and record the login event
                (await _data.Offices.GetAsync(officeUser.OfficeId, cancel))?.Login();
            }
        }

        _ = await _data.SaveEntitiesAsync(cancel);

        return user;
    }

    private async Task ProcessUserActionsAsync(
        User user,
        GetLoginUserInfoQuery query,
        string? mobilePhone,
        string? lastLoginIpAddress,
        CancellationToken cancel)
    {
        if (!query.IsSignUp)
        {
            _logger.LogInformation("User found. UserId: {Id}. Email: {Email}", user.Id, user.Email);
        }

        if (!string.IsNullOrEmpty(query.ProviderId))
        {
            user.ProcessUserInvitation(query.ProviderId, lastLoginIpAddress);
        }

        if (!query.IsMobileChange && !query.IsPwdChange)
        {
            user.Login(lastLoginIpAddress);
        }
        else if (query.IsMobileChange && !string.IsNullOrEmpty(mobilePhone))
        {
            user.UpdateMobilePhone(mobilePhone, lastLoginIpAddress);
        }

        if (query.IsPwdChange)
        {
            user.PwdChanged(query.FullName, lastLoginIpAddress);
        }

        _ = await _data.SaveEntitiesAsync(cancel);

        var @event = GetEventName(query);
        var log = new IdentityEventLogNotification(
            @event,
            _currentUser.Source,
            $"Id: {user.Id}. Name: {user.FullName}. Email: {user.Email}.",
            user.Id,
            user.DefaultOfficeId);
        await _mediator.Publish(log, cancel);
    }

    private async Task<AzureADUser?> GetAzureADUserAsync(GetLoginUserInfoQuery query, CancellationToken cancel)
    {
        if (!string.IsNullOrEmpty(query.ProviderId) && !query.IsTokenValidated)
        {
            try
            {
                return await _azureADProvider.GetUserAsync(query.ProviderId, cancel);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Azure AD user for UserId: {UserId}", query.ProviderId);
            }
        }
        return null;
    }

    private void QueueLastLoginSourceRefresh(int userId, string? providerId, bool isTokenValidated, bool shouldLogSession)
    {
        if (userId <= 0 || string.IsNullOrWhiteSpace(providerId) || isTokenValidated)
        {
            return;
        }

        _ = Task.Run(async () =>
            {
                using var fetchCts = new CancellationTokenSource(SignInInfoTimeout);
                try
                {
                    var signInInfo = await _azureADProvider.GetUserSignInInfoAsync(providerId, fetchCts.Token);
                    if (string.IsNullOrWhiteSpace(signInInfo.LastLoginIpAddress))
                    {
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IIdentityUnitOfWork>();
                    var repository = unitOfWork.Users;

                    var dbUser = await repository.FirstOrDefaultAsync(x => x.Id == userId, fetchCts.Token);
                    if (dbUser is null)
                    {
                        return;
                    }

                    dbUser.UpdateLastLoginSource(signInInfo.LastLoginIpAddress);
                    _ = await unitOfWork.SaveEntitiesAsync(fetchCts.Token);

                    if (shouldLogSession)
                    {
                        var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var sessionLogCommand = new UpsertSessionLogCommand(
                            SessionId: null,
                            Fingerprint: null,
                            OldSessionId: null,
                            Source: signInInfo.LastLoginIpAddress);
                        await scopedMediator.Send(sessionLogCommand, fetchCts.Token);
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogDebug(ex, "Cancelled Azure AD sign-in info retrieval for user {UserId}.", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to update last login source for user {UserId}.", userId);
                }
            });
    }

    private static string GetEventName(GetLoginUserInfoQuery query)
    {
        if (query.IsSignUp)
        {
            return "sign-up";
        }
        else if (query.IsMobileChange)
        {
            return "mobile change";
        }
        else if (query.IsPwdChange)
        {
            return "pwd change";
        }
        else if (query.IsTokenValidated)
        {
            return "token validated";
        }
        return "sign-in";
    }
}
