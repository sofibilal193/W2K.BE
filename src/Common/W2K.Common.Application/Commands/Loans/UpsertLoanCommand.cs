using W2K.Common.Models;
using MediatR;

namespace W2K.Common.Application.Commands.Loans;

public record UpsertLoanCommand(int LenderId, LoanInfo Loan) : IRequest<int?>;
