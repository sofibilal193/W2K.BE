using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using W2K.Common.Application.Auth;
using W2K.Common.Application.Settings;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Identity.Application.Notifications;
using W2K.Identity.Entities;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ValidateDevUserCommandHandler(
    IOptions<AppSettings> settingsOptions,
    IIdentityUnitOfWork data,
    IMediator mediator,
    ICurrentUser currentUser) : IRequestHandler<ValidateDevUserCommand, ValidateDevUserResponse>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMediator _mediator = mediator;
    private readonly AppSettings _settings = settingsOptions.Value;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ValidateDevUserResponse> Handle(ValidateDevUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevTokenSecretKey))
        {
            throw new DomainException("DevTokenSecretKey has not been set.");
        }
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevTokenIssuer))
        {
            throw new DomainException("DevTokenIssuer has not been set.");
        }
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevUser?.Email))
        {
            throw new DomainException("DevUser Email has not been set.");
        }
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevUser?.Password))
        {
            throw new DomainException("DevUser Password has not been set.");
        }
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevUser?.FirstName))
        {
            throw new DomainException("DevUser FirstName has not been set.");
        }
        if (string.IsNullOrEmpty(_settings.AuthSettings.DevUser?.LastName))
        {
            throw new DomainException("DevUser LastName has not been set.");
        }

        if (!_settings.AuthSettings.DevUser.HasValue
            || !(_settings.AuthSettings.DevUser.Value.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)
                    && _settings.AuthSettings.DevUser.Value.Password == request.Password))
        {
            return default;
        }

        var devUser = await _data.Users.Include("Offices.Office").Include("Offices.Role.Permissions").FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        if (devUser is null)
        {
            var devOffice = await _data.Offices
                .FirstOrDefaultAsync(x => x.Code == "devoffice", cancellationToken);

            var adminRole = await _data.Roles
                .FirstOrDefaultAsync(x => x.Name == "Administrator", cancellationToken);

            devUser = new User(
                providerId: "dev",
                firstName: _settings.AuthSettings.DevUser.Value.FirstName,
                lastName: _settings.AuthSettings.DevUser.Value.LastName,
                email: _settings.AuthSettings.DevUser.Value.Email.ToLowerInvariant(),
                mobilePhone: null,
                lastLoginIpAddress: _currentUser.Source
            );
            if (devOffice is not null)
            {
                devUser.UpsertOffice(
                    officeId: devOffice.Id,
                    roleId: adminRole?.Id,
                    title: null,
                    isDefault: true
                );
            }
            _data.Users.Add(devUser);
            _ = await _data.SaveEntitiesAsync(cancellationToken);
        }

        //Create a List of Claims, Keep claims name short
        var permClaims = new List<Claim>
            {
                new(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new(Common.Application.Identity.IdentityConstants.ObjectIdClaimTypeName, devUser.ProviderId ?? Guid.NewGuid().ToString()),
                new(ClaimTypes.Email, devUser.Email.ToLowerInvariant()),
                new(ClaimTypes.GivenName, devUser.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, devUser.LastName ?? string.Empty),
                new(ClaimTypes.Name, devUser.FullName),
                new("extension_UserId", devUser.Id.ToString()),
                new("extension_OfficeId", devUser.DefaultOfficeId?.ToString() ?? "0"),
                new("extension_OfficeType", devUser.DefaultOffice?.Office?.Type.ToString() ?? OfficeType.Merchant.ToString()),
                new("extension_OfficeName", devUser.DefaultOffice?.Office?.Name ?? OfficeType.Merchant.ToString()),
                new("extension_Role", devUser.DefaultOffice?.Role?.Name ?? "Administrator"),
                new("extension_OfficePermissions", string.Join("|", devUser.DefaultOffice?.Role?.Permissions?.Select(x => x.Name) ?? [])),
                new("extension_OfficeIds", string.Join('|', devUser.Offices.Select(x => x.OfficeId))),
                new("extension_IsSuperAdmin", devUser.DefaultOffice?.Office?.Type == OfficeType.SuperAdmin  ? "true" : "false"),
                new("extension_IsMultipleOffices", devUser.Offices.Count > 1 ? "true" : "false")
            };

        //Create Security Token object by giving required parameters
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AuthSettings.DevTokenSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _settings.AuthSettings.DevTokenIssuer, // Issuer
            _settings.AuthSettings.DevTokenIssuer,  //Audience
            permClaims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: credentials);
        var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

        // log event
        var log = new IdentityEventLogNotification(
            "dev user login",
            _currentUser.Source,
            $"Name: {devUser.FullName}. Email: {devUser.Email}.",
            _currentUser.UserId,
            _currentUser.OfficeIds?.FirstOrDefault());
        await _mediator.Publish(log, cancellationToken);

        return new ValidateDevUserResponse(jwt_token);
    }
}
