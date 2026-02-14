using W2K.Common.Application.DTOs.Files;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetLoanAppFilesQueryHandler(IFilesUnitOfWork data) : IRequestHandler<GetLoanAppFilesQuery, IReadOnlyCollection<FileDto>>
{
    private readonly IFilesUnitOfWork _data = data;

    public async Task<IReadOnlyCollection<FileDto>> Handle(GetLoanAppFilesQuery query, CancellationToken cancellationToken)
    {
        var files = await _data.Files
            .Include(x => x.Tags)
            .GetAsync(
                x => x.OfficeId == query.OfficeId && !x.IsDisabled
                        && x.Tags.Any(t => t.Name == FilesConstants.LoanAppId_FileTagName && t.Value == query.LoanAppId.ToString()),
                cancellationToken
            );

        return files.Select(x => new FileDto(x.Id, x.Label)).ToList();
    }
}
