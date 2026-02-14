using W2K.Common.Application.Extensions;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeBankAccountCommandValidator : AbstractValidator<UpsertOfficeBankAccountCommand>
{
    public UpsertOfficeBankAccountCommandValidator()
    {
        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        _ = RuleFor(x => x.AccountType)
            .IsInEnum();

        _ = RuleFor(x => x.BankName)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.AccountName)
            .NotEmpty()
            .MaximumLength(100);

        _ = RuleFor(x => x.RoutingNumber)
            .NotEmpty()
            .ValidRoutingNumber();

        _ = RuleFor(x => x.AccountNumber)
            .NotEmpty()
            .AccountNumber();

        _ = RuleFor(x => x.IsPrimary)
            .NotNull();
    }
}
