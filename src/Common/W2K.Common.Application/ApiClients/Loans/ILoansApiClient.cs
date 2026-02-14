using W2K.Common.Application.Commands.Loans;
using W2K.Common.Application.DTOs.Loans;
using W2K.Common.Application.Loans;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the Loans service.
/// </summary>
public interface ILoansApiClient
{
    /// <summary>
    /// Upserts a new loan in the Loans service.
    /// </summary>
    /// <param name="command">The upsert loan command.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task<int?> UpsertLoanAsync(UpsertLoanCommand command, CancellationToken cancel);

    /// <summary>
    /// Upserts the decision for a LoanApp.
    /// </summary>
    /// <param name="lenderId">The ID of the lender.</param>
    /// <param name="command">The command details.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task<int?> UpsertLoanAppDecisionAsync(UpsertLoanAppDecisionCommand command, CancellationToken cancel);

    /// <summary>
    /// Get Loan Details.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanId">The ID of the loan.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task<LoanDetailDto?> GetLoanDetailsAsync(int officeId, int loanId, CancellationToken cancel);


    /// <summary>
    /// Gets information about a loan application for webhook.
    /// </summary>
    /// <param name="lenderId">The ID of the lender.</param>
    /// <param name="lenderAppId">The lender's application identifier.</param>
    /// <param name="term">The loan term in months.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Loan application information if found; otherwise null.</returns>
    Task<LoanAppWebhookInfoDto?> GetLoanAppWebhookInfoAsync(int lenderId, string lenderAppId, int? term, CancellationToken cancel);

    /// <summary>
    /// Updates a loan with a new refund record.
    /// </summary>
    /// <param name="loanId">The ID of the loan.</param>
    /// <param name="command">The command containing refund details.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task UpdateLoanRefundAsync(int loanId, AddLoanRefundCommand command, CancellationToken cancel);
    /// Get Loan Application Details.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="loanAppId">The ID of the loan application.</param>
    /// <param name="cancel">Cancellation token.</param>
    Task<LoanAppDetailDto?> GetLoanAppDetailsAsync(int officeId, int loanAppId, CancellationToken cancel);
}
