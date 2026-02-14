using DFI.Common.Application.Lenders;
using DFI.Common.Models.Lenders;

namespace DFI.Common.Application.ApiClients;
/// <summary>
/// API client for communicating with the Lenders service.
/// </summary>
public interface ILendersApiClient
{
    /// <summary>
    /// Gets loan offers for a specific lender, office, and loan application.
    /// </summary>
    /// <param name="lenderId">The ID of the lender.</param>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="lenderAppId">The ID of the application in the lender's system.</param>
    /// <param name="amount">The loan amount.</param>
    /// <param name="serviceName">The service name (optional).</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>The offers response.</returns>
    Task<GetOffersResponse?> GetOffersAsync(int lenderId, int officeId, string lenderAppId, decimal amount, string? serviceName = null, CancellationToken cancel = default);

    /// <summary>
    /// Submits a loan application to the Lenders service.
    /// </summary>
    /// <param name="command">The submit loan app command.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>The submission response.</returns>
    Task<SubmitLoanAppResponse?> SubmitLoanAppAsync(SubmitLoanAppCommand command, CancellationToken cancel = default);

    /// <summary>
    /// Submits a refund request to the Lenders service.
    /// </summary>
    /// <param name="command">The command containing refund details.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>The refund response.</returns>
    Task<SubmitRefundResponse?> SubmitRefundAsync(int officeId, SubmitLenderRefundCommand command, CancellationToken cancel = default);
}
