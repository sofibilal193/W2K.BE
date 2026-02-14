using System.Text.Json.Serialization;
using W2K.Files.Application.Commands.FileTag;
using Microsoft.AspNetCore.Http;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class UpsertOfficeFilesCommand : IRequest<IEnumerable<int>>
{
    [JsonIgnore]
    public int OfficeId { get; private set; }

    public IEnumerable<FileCommand> Files { get; init; } = [];

    [JsonIgnore]
    public IReadOnlyCollection<IFormFile>? Documents { get; set; }

    public void SetOfficeId(int officeId)
    {
        OfficeId = officeId;
    }
}

public record FileCommand
{
    public string Label { get; init; } = string.Empty;

    [JsonIgnore]
    public string ContentType { get; private set; } = string.Empty;

    [JsonIgnore]
    public byte[] Content { get; private set; } = [];

    public IEnumerable<FileTagCommand> Tags { get; init; } = [];

    public void SetFile(string contentType, byte[] content)
    {
        Content = content;
        ContentType = contentType;
    }
}
