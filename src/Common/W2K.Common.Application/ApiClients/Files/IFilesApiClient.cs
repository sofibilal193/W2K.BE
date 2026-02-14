namespace DFI.Common.Application.ApiClients;

/// <summary>
/// API client for communicating with the Files service.
/// </summary>
public interface IFilesApiClient
{
    /// <summary>
    /// Soft deletes a file.
    /// </summary>
    /// <param name="officeId">The ID of the office.</param>
    /// <param name="fileId">The ID of the file.</param>
    /// <param name="cancel">Cancellation token.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    Task DeleteOfficeFileAsync(int officeId, int fileId, CancellationToken cancel = default);

}
