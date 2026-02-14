using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Validator for <see cref="GetUserByIdQuery"/> ensuring Id is a positive integer.
/// </summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        _ = RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User Id must be greater than zero.");
    }
}
