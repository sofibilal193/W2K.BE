using W2K.Common.Application.ApiServices;
using W2K.Common.Application.Lenders;
using W2K.Common.Models.Lenders;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client implementation for communicating with the Lenders service.
/// </summary>
public class LendersApiClient(IApiService apiService) : ILendersApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.Lenders;
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

    /// <inheritdoc />
    public async Task<GetOffersResponse?> GetOffersAsync(int lenderId, int officeId, string lenderAppId, decimal amount, string? serviceName = null, CancellationToken cancel = default)
    {
        var url = $"lenders/{lenderId}/offices/{officeId}/loanApps/offers?lenderAppId={lenderAppId}&amount={amount}";

        if (!string.IsNullOrWhiteSpace(serviceName))
        {
            url += $"&serviceName={serviceName}";
        }

        return await _apiService.GetAsync<GetOffersResponse?>(ServiceType, url, cancel);
    }

    /// <inheritdoc />
    public async Task<SubmitLoanAppResponse?> SubmitLoanAppAsync(SubmitLoanAppCommand command, CancellationToken cancel = default)
    {
        const string url = "lenders/submit";
        return await _apiService.PostAsync<SubmitLoanAppCommand, SubmitLoanAppResponse>(ServiceType, url, command, cancel);
    }

    /// <inheritdoc />
    public async Task<SubmitRefundResponse?> SubmitRefundAsync(int officeId, SubmitLenderRefundCommand command, CancellationToken cancel = default)
    {
        var url = $"lenders/offices/{officeId}/refund";
        return await _apiService.PostAsync<SubmitLenderRefundCommand, SubmitRefundResponse>(ServiceType, url, command, cancel);
    }
}
