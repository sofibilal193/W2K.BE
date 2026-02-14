using Microsoft.AspNetCore.Http;

namespace W2K.Common.Application.Validations;

public static class FileValidators
{
    public static bool HasMatchingDocuments<T, TFile>(
    this IEnumerable<TFile> files,
    T command,
    Func<T, IEnumerable<IFormFile>> getDocuments,
    Func<TFile, string> getLabel)
    {
        return files.All(x => getDocuments(command)?.Any(doc => doc.FileName == getLabel(x)) ?? false);
    }

    /// <summary>
    /// Check whether the file content length is within the allowed size.
    /// </summary>
    public static bool ValidFileSize(this byte[]? fileContent)
    {
        return fileContent is not null && fileContent.Length > 0 && fileContent.Length <= AppConstants.MaxFileSizeInBytes;
    }
}
