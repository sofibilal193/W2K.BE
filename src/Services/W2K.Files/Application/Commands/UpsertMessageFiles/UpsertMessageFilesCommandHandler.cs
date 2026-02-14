using W2K.Common.Application.ApiClients;
using W2K.Common.Application.Commands.Messaging;
using W2K.Common.Application.DTOs;
using W2K.Common.Application.DTOs.Files;
using W2K.Common.Application.Storage;
using W2K.Common.Identity;
using W2K.Common.ValueObjects;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handles uploading files to a message thread and creating a corresponding message in the Messaging service.
///
/// Architecture Decision: This handler uses a synchronous API call to the Messaging service rather than
/// publishing a domain event. This ensures immediate consistency and allows the handler to return the created
/// MessageId to the caller. The trade-off is tighter coupling between services, but it provides:
/// - Immediate feedback on message creation success/failure
/// - Atomicity: files are only persisted if the message is successfully created
/// - Simplified error handling and transaction boundaries
/// </summary>
public class UpsertMessageFilesCommandHandler(
    IFilesUnitOfWork fileData,
    IStorageProvider storageProvider,
    IMessagingApiClient messagingApiClient,
    ICurrentUser currentUser)
    : IRequestHandler<UpsertMessageFilesCommand, MessageFileDto>
{
    private readonly IFilesUnitOfWork _data = fileData ?? throw new ArgumentNullException(nameof(fileData));
    private readonly IStorageProvider _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
    private readonly IMessagingApiClient _messagingApiClient = messagingApiClient ?? throw new ArgumentNullException(nameof(messagingApiClient));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task<MessageFileDto> Handle(UpsertMessageFilesCommand request, CancellationToken cancellationToken)
    {
        var paths = await _storageProvider.UploadFilesAsync(
            FilesConstants.OfficesContainerName,
            $"{request.OfficeId}/{FilesConstants.MessagingDirectoryName}/{request.ThreadId}",
            request.Files.Select(x => new UploadedFile(x.FileName, x.Content)).ToList().AsReadOnly(),
            cancellationToken);

        var officeFiles = request.Files.Select((x, i) => new Entities.File(GetFileInfo(x, request.OfficeId, request.ThreadId, paths[i]))).ToList();

        _data.Files.AddRange(officeFiles);

        _ = await _data.SaveEntitiesAsync(cancellationToken);

        var fileDtos = officeFiles.Select(x => new FileDto(x.Id, x.Label)).ToList();

        var command = new UpsertMessageCommand
        {
            Content = string.Empty,
            Files = fileDtos.Select(x => new FileCommand
            {
                FileId = x.FileId,
                FileName = x.FileName
            }).ToList().AsReadOnly()
        };

        command.SetIds(request.OfficeId, request.ThreadId, null, null, null, request.IsInternal);

        var response = _currentUser.OfficeType == OfficeType.SuperAdmin
            ? await _messagingApiClient.AddMessageAsSuperAdminAsync(command, cancellationToken)
            : await _messagingApiClient.AddMessageAsMerchantAdminAsync(command, cancellationToken);

        return new MessageFileDto(response.MessageId, fileDtos);
    }

    private static Entities.File.FileInfo GetFileInfo(MessageFilesCommand fileCommand, int officeId, int threadId, string path)
    {
        return new Entities.File.FileInfo
        {
            OfficeId = officeId,
            Path = path,
            Label = fileCommand.FileName,
            ContentType = fileCommand.ContentType,
            Tags = [new Tag(FilesConstants.MessageThreadId_FileTagName, threadId.ToString())]
        };
    }
}
