using DFI.Common.Models;
using DFI.Common.Models.Lenders;
using MediatR;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace DFI.Common.Application.Lenders;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record SubmitLoanAppCommand : IRequest<SubmitLoanAppResponse>
{
    public required OfficeInfo Office { get; init; }

    public required LoanAppInfo LoanApp { get; init; }
}
