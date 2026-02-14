using W2K.Common.Application.ApiServices;
using W2K.Common.Application.Commands;

namespace W2K.Common.Application.ApiClients;

/// <summary>
/// API client implementation for communicating with the URL Shortener service.
/// </summary>
public class UrlShortenerApiClient(IApiService apiService) : IUrlShortenerApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.UrlShortener;
    private readonly IApiService _apiService = apiService;

    /// <inheritdoc />
    public async Task<string> ShortenUrlAsync(string LongUrl, CancellationToken cancel = default)
    {
        var url = "home";
        return await _apiService.PostAsync<ShortenUrlCommand, string>(ServiceType, url, new ShortenUrlCommand(LongUrl), cancel) ?? string.Empty;
    }
}
