using W2K.Common.Application.ApiServices;
using W2K.Common.Application.Commands.Loans;
using W2K.Common.Application.DTOs.Loans;
using W2K.Common.Application.Loans;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application.ApiClients;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// API client implementation for communicating with the Loans service.
/// </summary>

public class LoansApiClient(IApiService apiService) : ILoansApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.Loans;
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

    /// <inheritdoc />
    public async Task<int?> UpsertLoanAsync(UpsertLoanCommand command, CancellationToken cancel)
    {
        var url = "offices/loans";
        return await _apiService.PostAsync<UpsertLoanCommand, int?>(ServiceType, url, command, cancel);
    }

    /// <inheritdoc />
    public async Task<int?> UpsertLoanAppDecisionAsync(UpsertLoanAppDecisionCommand command, CancellationToken cancel)
    {
        var url = $"offices/lenders/{command.LenderId}/loanApps/decisions";
        return await _apiService.PutAsync<UpsertLoanAppDecisionCommand, int?>(ServiceType, url, command, cancel);
    }

    /// <inheritdoc />
    public async Task<LoanDetailDto?> GetLoanDetailsAsync(int officeId, int loanId, CancellationToken cancel)
    {
        var url = $"offices/{officeId}/loans/{loanId}";
        return await _apiService.GetAsync<LoanDetailDto?>(ServiceType, url, cancel);
    }

    /// <inheritdoc />
    public async Task<LoanAppWebhookInfoDto?> GetLoanAppWebhookInfoAsync(int lenderId, string lenderAppId, int? term, CancellationToken cancel)
    {
        var url = $"offices/lenders/{lenderId}/loanApps/webHookInfo?lenderAppId={Uri.EscapeDataString(lenderAppId)}&term={term}";
        return await _apiService.GetAsync<LoanAppWebhookInfoDto?>(ServiceType, url, cancel);
    }

    /// <inheritdoc />
    public async Task<LoanAppDetailDto?> GetLoanAppDetailsAsync(int officeId, int loanAppId, CancellationToken cancel)
    {
        var url = $"offices/{officeId}/loanApps/{loanAppId}";
        return await _apiService.GetAsync<LoanAppDetailDto?>(ServiceType, url, cancel);
    }

    /// <inheritdoc />
    public async Task UpdateLoanRefundAsync(int loanId, AddLoanRefundCommand command, CancellationToken cancel)
    {
        var url = $"offices/loans/{loanId}/refunds";
        await _apiService.PutAsync<AddLoanRefundCommand>(ServiceType, url, command, cancel);
    }
}
