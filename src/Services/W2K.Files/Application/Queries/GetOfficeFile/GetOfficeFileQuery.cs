using W2K.Files.Application.DTOs;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetOfficeFileQuery(int OfficeId, int FileId) : IRequest<OfficeFileDto?>;
