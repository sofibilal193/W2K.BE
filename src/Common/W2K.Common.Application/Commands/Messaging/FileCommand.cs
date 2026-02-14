
namespace DFI.Common.Application.Commands.Messaging;

public record FileCommand
{
    /// <summary>
    /// The ID of the file.
    /// </summary>
    public int FileId { get; init; }

    /// <summary>
    /// The name of the file.
    /// </summary>
    public string FileName { get; init; } = string.Empty;
}
