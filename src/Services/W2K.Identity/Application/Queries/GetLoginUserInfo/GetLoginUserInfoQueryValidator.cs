using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetLoginUserInfoQueryValidator : AbstractValidator<GetLoginUserInfoQuery>
{
    public GetLoginUserInfoQueryValidator()
    {
        _ = RuleFor(x => x.ProviderId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50);

        _ = RuleFor(x => x.FirstName)
            .NotEmpty()
            .When(x => x.IsSignUp)
            .MaximumLength(50);

        _ = RuleFor(x => x.LastName)
            .NotEmpty()
            .When(x => x.IsSignUp)
            .MaximumLength(50);

        _ = RuleFor(x => x.FullName)
            .MaximumLength(50);

        _ = RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .EmailAddress();

        _ = RuleFor(x => x.MobilePhone)
            .MaximumLength(20);

        _ = RuleFor(x => x.Step)
            .NotEmpty()
            .MaximumLength(20);
    }
}
