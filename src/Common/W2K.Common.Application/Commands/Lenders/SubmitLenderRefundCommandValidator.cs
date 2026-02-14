using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application.Lenders;

public class SubmitLenderRefundCommandValidator : AbstractValidator<SubmitLenderRefundCommand>
{
    public SubmitLenderRefundCommandValidator()
    {
        _ = RuleFor(x => x.LenderId)
            .GreaterThan(0);

        _ = RuleFor(x => x.OfficeId)
            .GreaterThan(0);

        _ = RuleFor(x => x.LoanId)
            .GreaterThan(0);

        _ = RuleFor(x => x.LenderAppId)
            .NotEmpty();

        _ = RuleFor(x => x.Amount)
            .GreaterThan(0);

        _ = RuleFor(x => x.Reason)
            .NotEmpty();
    }
}
