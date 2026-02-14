using W2K.Common.Application.DTOs.Files;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public readonly record struct GetLoanFilesQuery(int OfficeId, int LoanId) : IRequest<IReadOnlyCollection<FileDto>>;
