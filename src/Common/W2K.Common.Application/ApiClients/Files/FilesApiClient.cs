using W2K.Common.Application.ApiServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Common.Application.ApiClients;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// API client implementation for communicating with the Files service.
/// </summary>
public class FilesApiClient(IApiService apiService) : IFilesApiClient
{
    private static readonly string ServiceType = ApiServiceTypes.Files;
    private readonly IApiService _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));

    /// <inheritdoc />
    public async Task DeleteOfficeFileAsync(int officeId, int fileId, CancellationToken cancel = default)
    {
        var url = $"offices/{officeId}/files/{fileId}";
        await _apiService.DeleteAsync(ServiceType, url, cancel);
    }
}
