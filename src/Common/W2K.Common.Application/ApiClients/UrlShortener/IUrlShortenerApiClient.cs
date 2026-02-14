namespace DFI.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the URL Shortener service.
/// </summary>
public interface IUrlShortenerApiClient
{
    /// <summary>
    /// Shortens a given long URL and returns the shortened URL key.
    /// </summary>
    /// <param name="LongUrl">The original long URL to shorten.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>The shortened URL key.</returns>
    Task<string> ShortenUrlAsync(string LongUrl, CancellationToken cancel = default);
}
