using W2K.Common.Application.Extensions;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class AdminUserCommandValidator : AbstractValidator<AdminUserCommand>
{
    public AdminUserCommandValidator()
    {
        _ = RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(25);

        _ = RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(25);

        _ = RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        _ = RuleFor(x => x.MobilePhone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .PhoneNumber();
    }
}
