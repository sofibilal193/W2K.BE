using W2K.Common.Application.DTOs.Files;
using W2K.Common.Application.Storage;
using W2K.Common.ValueObjects;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertLoanAppFilesCommandHandler(
    IFilesUnitOfWork fileData,
    IStorageProvider storageProvider)
    : IRequestHandler<UpsertLoanAppFilesCommand, IReadOnlyCollection<FileDto>>
{
    private readonly IFilesUnitOfWork _data = fileData ?? throw new ArgumentNullException(nameof(fileData));
    private readonly IStorageProvider _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));

    public async Task<IReadOnlyCollection<FileDto>> Handle(UpsertLoanAppFilesCommand request, CancellationToken cancellationToken)
    {
        var paths = await _storageProvider.UploadFilesAsync(
            FilesConstants.OfficesContainerName,
            $"{request.OfficeId}/{FilesConstants.LoanAppsDirectoryName}/{request.LoanAppId}",
            request.Files.Select(x => new UploadedFile(x.FileName, x.Content)).ToList().AsReadOnly(),
            cancellationToken);

        var files = request.Files.Select((x, i) => new Entities.File(GetFileInfo(x, request.OfficeId, request.LoanAppId, paths[i]))).ToList();

        _data.Files.AddRange(files);

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        return files.Select(x => new FileDto(x.Id, x.Label)).ToList();
    }

    private static Entities.File.FileInfo GetFileInfo(LoanAppFileCommand fileCommand, int officeId, int loanAppId, string path)
    {
        return new Entities.File.FileInfo
        {
            OfficeId = officeId,
            Path = path,
            Label = fileCommand.FileName,
            ContentType = fileCommand.ContentType,
            Tags = [new Tag(FilesConstants.LoanAppId_FileTagName, loanAppId.ToString())]
        };
    }
}
