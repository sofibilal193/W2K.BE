using W2K.Common.Application.Storage;
using W2K.Common.Exceptions;
using W2K.Common.Identity;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class DeleteOfficeFilesCommandHandler(IFilesUnitOfWork fileData, IStorageProvider storageProvider, ICurrentUser currentUser, IMediator mediator) : IRequestHandler<DeleteOfficeFilesCommand>
{
    private readonly IFilesUnitOfWork _data = fileData ?? throw new ArgumentNullException(nameof(fileData));
    private readonly IStorageProvider _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Handle(DeleteOfficeFilesCommand command, CancellationToken cancellationToken)
    {
        // Get the file entity from DB
        var file = await _data.Files
                    .Include(x => x.Tags)
                    .FirstOrDefaultAsync(
                        x => x.OfficeId == command.OfficeId && x.Id == command.FileId,
                        cancellationToken)
            ?? throw new NotFoundException(nameof(Entities.File), command.FileId);

        // Validate file path integrity
        if (string.IsNullOrWhiteSpace(file.Path))
        {
            throw new InvalidOperationException($"File {file.Id} has invalid path and cannot be deleted");
        }

        // Delete from Azure Blob Storage
        var isDeleted = await _storageProvider.DeleteFileAsync(file.Path, cancellationToken);

        if (!isDeleted)
        {
            throw new InvalidOperationException($"Failed to delete file from storage. File ID: {file.Id}");
        }

        // Delete from database only after blob deletion succeeds
        _data.Files.Remove(file);
        _ = await _data.SaveEntitiesAsync(cancellationToken);

        // Publish notification after successful deletion
        var notification = new Notifications.FilesEventLogNotification(
            "Office File Deleted",
            _currentUser.Source,
            $"File with ID: {file.Id} and label: {file.Label} was deleted.",
            _currentUser.UserId,
            file.OfficeId,
            file.Id);
        await _mediator.Publish(notification, cancellationToken);

    }
}
