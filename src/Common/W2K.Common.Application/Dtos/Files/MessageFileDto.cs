namespace DFI.Common.Application.DTOs.Files;

public record MessageFileDto
{
    /// <summary>
    /// The ID of message.
    /// </summary>
    public Guid MessageId { get; init; }

    /// <summary>
    /// Uploaded files.
    /// </summary>
    public IEnumerable<FileDto> Files { get; init; } = Array.Empty<FileDto>();

    public MessageFileDto(Guid messageId, IEnumerable<FileDto> files)
    {
        MessageId = messageId;
        Files = files;
    }
}
