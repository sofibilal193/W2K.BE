using System.Collections.ObjectModel;
using W2K.Common.Entities;
using W2K.Common.ValueObjects;

namespace W2K.Files.Entities;

public class File : BaseEntity
{
    #region Private Fields

    private readonly List<Tag> _tags;

    #endregion

    #region Properties

    public int? OfficeId { get; private set; }

    public string Label { get; private set; } = "";

    public string Path { get; private set; } = "";

    public string ContentType { get; private set; } = "";

    #endregion

    #region Owned Entities

    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    #endregion

    public File(FileInfo fileInfo)
        : this()
    {
        OfficeId = fileInfo.OfficeId;
        Path = fileInfo.Path;
        Label = fileInfo.Label;
        ContentType = fileInfo.ContentType;
        if (fileInfo.Tags is not null)
        {
            _tags.AddRange(fileInfo.Tags);
        }
    }

    protected File()
    {
        _tags = [];
    }

    public readonly record struct FileInfo(
        int OfficeId,
        string Path,
        string Label,
        string ContentType,
        ReadOnlyCollection<Tag> Tags);
}
