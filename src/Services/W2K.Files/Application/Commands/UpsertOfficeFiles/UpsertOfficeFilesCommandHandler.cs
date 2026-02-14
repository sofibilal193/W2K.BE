using System.Collections.ObjectModel;
using W2K.Common.Application.Storage;
using W2K.Common.Identity;
using W2K.Common.ValueObjects;
using W2K.Files.Application.Notifications;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeFilesCommandHandler(IFilesUnitOfWork fileData, IStorageProvider storageProvider, ICurrentUser currentUser, IMediator mediator) : IRequestHandler<UpsertOfficeFilesCommand, IEnumerable<int>>
{
    private readonly IFilesUnitOfWork _data = fileData;
    private readonly IStorageProvider _storageProvider = storageProvider;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IMediator _mediator = mediator;

    public async Task<IEnumerable<int>> Handle(UpsertOfficeFilesCommand command, CancellationToken cancellationToken)
    {
        var files = command.Files.Select(x => new UploadedFile(x.Label, x.Content!)).ToList().AsReadOnly();

        var paths = await _storageProvider.UploadFilesAsync(FilesConstants.OfficesContainerName, $"{command.OfficeId}", files, cancellationToken);

        if (paths is null || paths.Count == 0)
        {
            return [];
        }

        var newFileIds = new List<int>();
        for (int i = 0; i < command.Files.Count(); i++)
        {
            var uploadedFile = command.Files.ElementAt(i);
            var path = paths[i];
            var officeFile = new Entities.File(GetFileInfo(uploadedFile, command.OfficeId, path));
            _data.Files.Add(officeFile);
            _ = await _data.SaveEntitiesAsync(cancellationToken);
            newFileIds.Add(officeFile.Id);

            // Publish notification after file is uploaded.
            var notification = new FilesEventLogNotification(
                "Office File Uploaded",
                _currentUser.Source,
                $"File with ID: {officeFile.Id} was uploaded.",
                _currentUser.UserId,
                command.OfficeId,
                officeFile.Id);
            await _mediator.Publish(notification, cancellationToken);
        }
        return newFileIds;
    }

    private static Entities.File.FileInfo GetFileInfo(FileCommand fileCommand, int officeId, string path)
    {
        return new Entities.File.FileInfo
        {
            OfficeId = officeId,
            Path = path,
            Label = fileCommand.Label,
            ContentType = fileCommand.ContentType,
            Tags = GetTags(fileCommand)
        };
    }

    private static ReadOnlyCollection<Tag> GetTags(FileCommand fileCommand)
    {
        if (!fileCommand.Tags.Any())
        {
            return new ReadOnlyCollection<Tag>(Array.Empty<Tag>());
        }

        // Map each FileTagCommand to a Tag object
        return new ReadOnlyCollection<Tag>(
            [.. fileCommand.Tags
                .Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.Value))
                .Select(x => new Tag(x.Name, x.Value))]
        );
    }
}
