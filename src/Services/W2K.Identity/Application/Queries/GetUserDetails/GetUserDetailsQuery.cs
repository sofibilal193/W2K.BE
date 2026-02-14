using W2K.Identity.Application.DTOs;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Query to retrieve comprehensive user details including personal information,
/// account status, and all associated office roles.
/// </summary>
/// <param name="UserId">The ID of the user.</param>
public readonly record struct GetUserDetailsQuery(int UserId) : IRequest<UserDetailsDto>;


public class GetUserDetailsQueryValidator : AbstractValidator<GetUserDetailsQuery>
{
    public GetUserDetailsQueryValidator()
    {
        _ = RuleFor(x => x.UserId)
            .GreaterThan(0);
    }
}
