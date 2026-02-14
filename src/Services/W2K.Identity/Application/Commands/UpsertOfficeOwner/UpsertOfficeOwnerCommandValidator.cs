using W2K.Common.Application.Commands;
using W2K.Common.Application.Extensions;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeOwnerCommandValidator : AbstractValidator<UpsertOfficeOwnerCommand>
{
    public UpsertOfficeOwnerCommandValidator()
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        _ = RuleForEach(x => x.Owners)
            .ChildRules(x =>
                {
                    _ = x.RuleFor(x => x.FirstName)
                        .NotEmpty()
                        .MaximumLength(25);

                    _ = x.RuleFor(x => x.LastName)
                        .NotEmpty()
                        .MaximumLength(25);

                    _ = x.RuleFor(x => x.Email)
                        .NotEmpty()
                        .EmailAddress();

                    _ = x.RuleFor(x => x.MobilePhone)
                        .Cascade(CascadeMode.Stop)
                        .NotEmpty()
                        .PhoneNumber();

                    _ = x.RuleFor(x => x.DOB)
                        .NotEmpty()
                        .DateOfBirth();

                    _ = x.RuleFor(x => x.SSN)
                        .NotEmpty()
                        .SocalSecurityNumber();

                    _ = x.RuleFor(x => x.Ownership)
                        .NotEmpty()
                        .InclusiveBetween((short)0, (short)100);

                    _ = x.RuleFor(x => x.AnnualIncome)
                        .GreaterThan(0)
                        .When(x => x.AnnualIncome.HasValue);

                    _ = x.RuleFor(x => x.NetWorth)
                        .GreaterThan(0)
                        .When(x => x.NetWorth.HasValue);

                    _ = x.RuleFor(x => x.Address)
                        .NotNull()
                        .SetValidator(new AddressCommandValidator());
                });

        _ = RuleFor(x => x.Owners.Sum(o => o.Ownership))
            .Equal(100)
            .WithMessage("Total ownership across all owners must be exactly 100%.");
    }
}
