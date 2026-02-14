using W2K.Common.Application.ApiServices;
using W2K.Common.Application.Auth;
using W2K.Common.Application.DTOs;
using Microsoft.AspNetCore.WebUtilities;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client implementation for communicating with the Identity service.
/// </summary>
public class IdentityApiClient(IApiService apiService) : IIdentityApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.Identity;
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

    /// <inheritdoc />
    public async Task<OfficeDto> GetOfficeDetailsAsync(int officeId, CancellationToken cancel = default)
    {
        var url = $"offices/{officeId}";
        return await _apiService.GetAsync<OfficeDto?>(ServiceType, url, cancel) ?? new();
    }

    /// <inheritdoc />
    public async Task<List<LenderDto>> GetLendersAsync(CancellationToken cancel = default)
    {
        var url = "lenders";
        return await _apiService.GetAsync<List<LenderDto>>(ServiceType, url, cancel) ?? [];
    }

    /// <inheritdoc />
    public async Task<IList<string>> GetUserPermissionsAsync(int? userId, int? officeId, CancellationToken cancel = default)
    {
        var url = "permissions";
        var queryParams = new Dictionary<string, string?>();
        if (userId.HasValue)
        {
            queryParams.Add("userId", userId.Value.ToString());
        }
        if (officeId.HasValue && officeId > 0)
        {
            queryParams.Add("officeId", officeId.Value.ToString());
        }
        if (queryParams.Count > 0)
        {
            url = QueryHelpers.AddQueryString(url, queryParams);
        }

        return await _apiService.GetAsync<IList<string>>(ServiceType, url, cancel) ?? [];
    }

    /// <inheritdoc />
    public async Task<bool> ValidateOfficeUserAsync(int officeId, CancellationToken cancel = default)
    {
        var url = $"auth/{AuthConstants.OfficeAccessRouteValue}?officeId={officeId}";
        return await _apiService.GetAsync<bool>(ServiceType, url, cancel);
    }
}
