using W2K.Common.Application.Extensions;
using W2K.Identity.Repositories;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeUserCommandValidator : AbstractValidator<UpsertOfficeUserCommand>
{
    public UpsertOfficeUserCommandValidator(IIdentityUnitOfWork _data)
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        _ = RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(25);

        _ = RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(25);

        _ = RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();

        _ = RuleFor(x => x.Email)
            .EmailMustBeUniqueInDatabase(async (email, cancel) =>
                {
                    return await _data.Users.AnyAsync(u => u.Email == email, cancel);
                })
            .When(x => !x.UserId.HasValue);

        _ = RuleFor(x => x.MobilePhone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .PhoneNumber();

        _ = RuleFor(x => x.Title)
            .MaximumLength(25)
            .NotEmpty()
            .When(x => x.OfficeId.HasValue);

        _ = RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .When(x => x.OfficeId.HasValue);
    }
}

