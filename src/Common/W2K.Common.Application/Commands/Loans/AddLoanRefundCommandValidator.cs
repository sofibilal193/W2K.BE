using DFI.Common.Application.Extensions;
using FluentValidation;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace DFI.Common.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Validator for the AddLoanRefundCommand.
/// </summary>
public class AddLoanRefundCommandValidator : AbstractValidator<AddLoanRefundCommand>
{
    public AddLoanRefundCommandValidator()
    {
        RuleFor(x => x.LoanId)
            .GreaterThan(0);

        RuleFor(x => x.RefundAmount)
            .GreaterThan(0);

        RuleFor(x => x.NetRefundAmount)
            .GreaterThan(0);

        RuleFor(x => x.NetOfficeRefundAmount)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MustBeBase64Encoded(255);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
