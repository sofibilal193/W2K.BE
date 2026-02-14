using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DFI.Common.Application.Storage;
using DFI.Common.Infrastructure.Settings;
using DFI.Common.Files;
using System.Collections.ObjectModel;
using DFI.Common.Application.Extensions;
using Microsoft.Extensions.Options;

namespace DFI.Common.Infrastructure.Storage;

/// <summary>
/// An implementation of <see cref="IStorageProvider"/> that uses Azure Blob Storage
/// </summary>
public class AzureStorageProvider(IOptions<AzureStorageSettings> settings, IHostEnvironment env) : IStorageProvider
{
    #region Private Properties

    private readonly AzureStorageSettings _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

    private readonly string _invalidFileNameChars = @"[~#%&\*\{\}\(\)\\/:;<>\?\|""'\^]";
    private readonly string _invalidAsciiChars = @"[^\u0000-\u007F]+";
    private readonly IHostEnvironment _env = env ?? throw new ArgumentNullException(nameof(env));

    private readonly List<BlobContainerClient> _containers = [];

    #endregion

    #region IStorageProvider Methods

    public async Task<string?> UploadFileAsync(string containerName, string prefix, string contentType, byte[] data, CancellationToken cancel = default)
    {
        string? filePath = null;

        List<UploadedFile> files =
        [
            new UploadedFile(Guid.NewGuid().ToString() + contentType.GetFileExtension(), data)
        ];

        var paths = await UploadFilesAsync(containerName, prefix, files.AsReadOnly(), cancel);
        if (paths is not null && paths.Count > 0)
        {
            filePath = paths[0];
        }
        return filePath;
    }

    public async Task<ReadOnlyCollection<string>> UploadFilesAsync(string containerName, ReadOnlyCollection<UploadedFile> files, CancellationToken cancel = default)
    {
        return await UploadFilesAsync(containerName, null, files, cancel);
    }

    public async Task<ReadOnlyCollection<string>> UploadFilesAsync(string containerName, string? prefix, ReadOnlyCollection<UploadedFile> files, CancellationToken cancel = default)
    {
        List<string> filePaths = [];

        // Get a reference to the container
        var container = await GetBlobContainerAsync(containerName, cancel);

        foreach (var file in files)
        {
            // Retrieve reference to a blob
            string fileName = string.IsNullOrEmpty(file.Name) ? Guid.NewGuid().ToString() : CleanFileName(file.Name);
            string blobName = string.IsNullOrEmpty(prefix) ? fileName : prefix.ToLowerInvariant() + "/" + fileName;
            var blob = container.GetBlobClient(blobName);
            if (blob is not null)
            {
                if (file.Data is not null)
                {
                    using var ms = new MemoryStream(file.Data, false);
                    _ = await blob.UploadAsync(ms, overwrite: true, cancel);
                }
                // Store relative path: containerName/blobName
                string relativePath = $"{containerName}/{blobName}";
                filePaths.Add(relativePath);

                // set blob metadata
                if (file.MetaData is not null)
                {
                    _ = await blob.SetMetadataAsync(file.MetaData, cancellationToken: cancel);
                }

                if (file.TtlDays.HasValue)
                {
                    _ = await blob.SetTagsAsync(new Dictionary<string, string> { { "TTL", file.TtlDays.Value.ToString() } }, cancellationToken: cancel);
                }
            }
        }

        return filePaths.AsReadOnly();
    }

    public async Task<UploadedFile?> GetFileAsync(string path, CancellationToken cancel = default)
    {
        var files = await GetFilesAsync(new ReadOnlyCollection<string>([path]), cancel);
        return files?.FirstOrDefault();
    }

    public async Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(ReadOnlyCollection<string> paths, CancellationToken cancel = default)
    {
        List<UploadedFile> list = [];

        foreach (var path in paths)
        {
            var containerBlob = GetContainerNameFromUri(path);
            if (!string.IsNullOrEmpty(containerBlob.ContainerName) && !string.IsNullOrEmpty(containerBlob.BlobName))
            {
                // retrieve reference to container
                var container = await GetBlobContainerAsync(containerBlob.ContainerName, cancel);
                var file = await GetFileFromBlobAsync(container, containerBlob.BlobName, cancel);
                if (file is not null)
                {
                    list.Add(file);
                }
            }
        }

        return list.AsReadOnly();
    }

    public async Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(string containerName, CancellationToken cancel = default)
    {
        return await GetFilesAsync(containerName, null, cancel);
    }

    public async Task<ReadOnlyCollection<UploadedFile>> GetFilesAsync(string containerName, string? prefix, CancellationToken cancel = default)
    {
        List<UploadedFile> list = [];

        // Retrieve a reference to a container.
        var container = await GetBlobContainerAsync(containerName, cancel);

        // Call the listing operation and return pages of the specified size.
        var resultSegment = container.GetBlobsAsync(
                traits: BlobTraits.None,
                states: BlobStates.None,
                prefix: prefix,
                cancellationToken: cancel)
            .AsPages();

        // Enumerate the blobs returned for each page.
        await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
        {
            foreach (BlobItem blobItem in blobPage.Values)
            {
                var file = await GetFileFromBlobAsync(container, blobItem.Name, cancel);
                if (file is not null)
                {
                    list.Add(file);
                }
            }
        }
        return list.AsReadOnly();
    }

    public async Task<ReadOnlyCollection<UploadedFile>> GetFilesListAsync(string containerName, CancellationToken cancel = default)
    {
        return await GetFilesListAsync(containerName, null, cancel);
    }

    public async Task<ReadOnlyCollection<UploadedFile>> GetFilesListAsync(string containerName, string? prefix, CancellationToken cancel = default)
    {
        List<UploadedFile> list = [];

        // Retrieve a reference to a container.
        var container = await GetBlobContainerAsync(containerName, cancel);

        // Call the listing operation and return pages of the specified size.
        var resultSegment = container.GetBlobsAsync(
                prefix: prefix,
                traits: BlobTraits.All,
                states: BlobStates.None,
                cancellationToken: cancel)
            .AsPages();

        // Enumerate the blobs returned for each page.
        await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
        {
            foreach (BlobItem blobItem in blobPage.Values)
            {
                // Store relative path: containerName/blobName
                var relativePath = $"{containerName}/{blobItem.Name}";
                var file = new UploadedFile(relativePath, blobItem.Name, blobItem.Metadata);
                list.Add(file);
            }
        }
        return list.AsReadOnly();
    }

    public async Task<bool> SetFileMetaDataAsync(string path, IDictionary<string, string> metaData, CancellationToken cancel = default)
    {
        // Retrieve a reference to a container.
        var containerBlob = GetContainerNameFromUri(path);
        if (!string.IsNullOrEmpty(containerBlob.ContainerName) && !string.IsNullOrEmpty(containerBlob.BlobName))
        {
            var container = await GetBlobContainerAsync(containerBlob.ContainerName, cancel);

            // Retrieve reference to a blob
            var blockBlob = container.GetBlobClient(containerBlob.BlobName);
            if (blockBlob is not null)
            {
                _ = await blockBlob.SetMetadataAsync(metaData, cancellationToken: cancel);
            }
        }
        return true;
    }

    public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancel = default)
    {
        // Get a reference to the container
        var containerBlob = GetContainerNameFromUri(fileName);
        var container = await GetBlobContainerAsync(containerBlob.ContainerName ?? _settings.DefaultContainerName, cancel);
        var blob = container.GetBlobClient(containerBlob.BlobName);
        _ = await blob.DeleteAsync(cancellationToken: cancel);
        return true;
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // dispose resources
    }

    #endregion

    #region Private Methods

    private static async Task<UploadedFile?> GetFileFromBlobAsync(BlobContainerClient container, string blobName, CancellationToken cancel = default)
    {
        UploadedFile? file = null;
        // Retrieve reference to a blob
        var blockBlob = container.GetBlobClient(blobName);
        if (await blockBlob.ExistsAsync(cancel))
        {
            // Store relative path: containerName/blobName
            var relativePath = $"{container.Name}/{blobName}";
            file = new UploadedFile(relativePath, blobName);
            using (var memoryStream = new MemoryStream())
            {
                _ = await blockBlob.DownloadToAsync(memoryStream, cancel);
                file.SetData(memoryStream.ToArray());
            }

            // retrieve metadata
            var props = await blockBlob.GetPropertiesAsync(cancellationToken: cancel);
            file.SetMetaData(props?.Value?.Metadata);
        }
        return file;
    }

    private string CleanFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var ext = Path.GetExtension(fileName);
        // Replace invalid characters with empty strings.
        var returnstr = Regex.Replace(
            Regex.Replace(name, _invalidFileNameChars, "", RegexOptions.NonBacktracking),
            _invalidAsciiChars,
            "",
            RegexOptions.NonBacktracking);
        returnstr += ext;
        return returnstr;
    }

    private BlobClientOptions? GetBlobClientOptions()
    {
        if (_settings.RetryOptions is null)
        {
            return null;
        }
        else
        {
            Uri? geoRedundantSecondaryUri = string.IsNullOrEmpty(_settings.GeoRedundantSecondaryUri)
                ? null
                : new Uri(_settings.GeoRedundantSecondaryUri);
            return new BlobClientOptions
            {
                Retry = {
                        Delay = _settings.RetryOptions.Delay,
                        MaxDelay = _settings.RetryOptions.MaxDelay,
                        MaxRetries = _settings.RetryOptions.MaxRetries,
                        Mode = _settings.RetryOptions.Mode,
                        NetworkTimeout = _settings.RetryOptions.NetworkTimeout
                    },
                GeoRedundantSecondaryUri = geoRedundantSecondaryUri
            };
        }
    }

    private BlobServiceClient GetBlobServiceClient()
    {
        // Create a BlobServiceClient that will authenticate through Active Directory
        if (_env.IsDevelopment() || _env.IsTest())
        {
            return new(_settings.PrimaryUri, GetBlobClientOptions());
        }
        else
        {
            // PrimaryUri already contains the base storage URL (e.g., https://<account>.blob.core.windows.net/)
            var accountUri = new Uri(_settings.PrimaryUri);
            return new(accountUri, new DefaultAzureCredential(), GetBlobClientOptions());
        }
    }

    private static ContainerBlobName GetContainerNameFromUri(string path)
    {
        // Handle relative path format: containerName/prefix/fileName or containerName/fileName
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var containerName = segments[0];
        var blobName = string.Join("/", segments.Skip(1));

        return new ContainerBlobName(containerName, blobName);
    }

    private async Task<BlobContainerClient> GetBlobContainerAsync(string containerName, CancellationToken cancel)
    {
        var container = _containers.Find(x => x.Name == containerName);
        if (container is null)
        {
            var client = GetBlobServiceClient();
            container = client.GetBlobContainerClient(containerName);
            _ = await container.CreateIfNotExistsAsync(cancellationToken: cancel);
            _containers.Add(container);
        }
        return container;
    }

    #endregion

    private sealed class ContainerBlobName(string? containerName, string? blobName)
    {
        public string? ContainerName { get; set; } = containerName;
        public string? BlobName { get; set; } = blobName;
    }
}
