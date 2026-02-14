using AutoMapper;
using W2K.Common.Exceptions;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles retrieval of comprehensive user details including associated office roles.
/// Validates user existence and returns 404 if not found.
/// </summary>
public class GetUserDetailsQueryHandler(
    IIdentityUnitOfWork data,
    IMapper mapper) : IRequestHandler<GetUserDetailsQuery, UserDetailsDto>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<UserDetailsDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _data.Users
                    .AsNoTracking()
                    .Include("Offices.Office")
                    .Include("Offices.Role")
                    .GetAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException($"User with ID {request.UserId} not found.");
        return _mapper.Map<UserDetailsDto>(user);
    }
}
