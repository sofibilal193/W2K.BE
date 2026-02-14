using W2K.Common.Application.Extensions;
using W2K.Identity.Repositories;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertSuperAdminUserCommandValidator : AbstractValidator<UpsertSuperAdminUserCommand>
{
    public UpsertSuperAdminUserCommandValidator(IIdentityUnitOfWork data)
    {
        _ = RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        _ = RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        _ = RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .EmailAddress();

        _ = RuleFor(x => x.Email)
            .EmailMustBeUniqueInDatabase(async (email, cancel) =>
                {
                    return await data.Users.AnyAsync(u => u.Email == email, cancel);
                })
            .When(x => !x.UserId.HasValue);

        _ = RuleFor(x => x.MobilePhone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .PhoneNumber();

        _ = RuleFor(x => x.Title)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Title));

        _ = RuleFor(x => x.RoleId)
            .GreaterThan(0);
    }
}
