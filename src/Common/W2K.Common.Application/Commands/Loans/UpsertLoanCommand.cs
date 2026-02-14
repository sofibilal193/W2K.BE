using DFI.Common.Models;
using MediatR;

namespace DFI.Common.Application.Commands.Loans;

public record UpsertLoanCommand(int LenderId, LoanInfo Loan) : IRequest<int?>;
