#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Commands;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct DeleteOfficeFilesCommand(int OfficeId, int FileId) : IRequest;
