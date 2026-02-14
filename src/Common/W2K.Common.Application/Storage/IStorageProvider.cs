using System.Collections.ObjectModel;

namespace W2K.Common.Application.Storage;

public interface IStorageProvider : IDisposable
{
    Task<string?> UploadFileAsync(
        string containerName,
        string prefix,
        string contentType,
        byte[] data,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<string>> UploadFilesAsync(
        string containerName,
        ReadOnlyCollection<UploadedFile> files,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<string>> UploadFilesAsync(
        string containerName,
        string prefix,
        ReadOnlyCollection<UploadedFile> files,
        CancellationToken cancel = default);

    Task<UploadedFile?> GetFileAsync(
        string path,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(
        ReadOnlyCollection<string> paths,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(
        string containerName,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(
        string containerName,
        string prefix,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<UploadedFile>> GetFilesListAsync(
        string containerName,
        CancellationToken cancel = default);

    Task<ReadOnlyCollection<UploadedFile>> GetFilesListAsync(
        string containerName,
        string prefix,
        CancellationToken cancel = default);

    Task<bool> SetFileMetaDataAsync(
        string path,
        IDictionary<string, string> metaData,
        CancellationToken cancel = default);

    Task<bool> DeleteFileAsync(
        string fileName,
        CancellationToken cancel = default);
}
