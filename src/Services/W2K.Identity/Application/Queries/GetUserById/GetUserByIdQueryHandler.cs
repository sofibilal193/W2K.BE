using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles retrieval of a user by Id. Returns only basic identity fields.
/// </summary>
public class GetUserByIdQueryHandler(
    IIdentityUnitOfWork data,
    ILogger<GetUserByIdQueryHandler> logger) : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _data.Users.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (user is null)
        {
            _logger.LogInformation("No user found for Id: {Id}", request.Id);
            return null;
        }
        return new UserDto
        {
            Id = user.Id,
            ProviderId = user.ProviderId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            MobilePhone = user.MobilePhone,
            LastLoginDateTimeUtc = user.LastLoginDateTimeUtc,
            LastLogoutDateTimeUtc = user.LastLogoutDateTimeUtc
        };
    }
}
