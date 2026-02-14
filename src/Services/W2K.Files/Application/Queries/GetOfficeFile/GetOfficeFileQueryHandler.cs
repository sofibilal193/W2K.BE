using AutoMapper;
using W2K.Common.Application.Storage;
using W2K.Common.Exceptions;
using W2K.Files.Application.DTOs;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeFileQueryHandler(IFilesUnitOfWork data, IMapper mapper, IStorageProvider storageProvider) : IRequestHandler<GetOfficeFileQuery, OfficeFileDto?>
{
    private readonly IFilesUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;
    private readonly IStorageProvider _storageProvider = storageProvider;

    public async Task<OfficeFileDto?> Handle(GetOfficeFileQuery query, CancellationToken cancellationToken)
    {
        var file = await _data.Files.Include(x => x.Tags).FirstOrDefaultAsync(x => x.OfficeId == query.OfficeId && x.Id == query.FileId, cancellationToken)
            ?? throw new NotFoundException(nameof(Entities.File), query.FileId);

        // Get the file from storage
        if (!string.IsNullOrEmpty(file.Path))
        {
            var uploadedFile = await _storageProvider.GetFileAsync(file.Path, cancellationToken);

            var officeFileDto = _mapper.Map<OfficeFileDto>(uploadedFile) with
            {
                FileType = file.ContentType
            };

            return officeFileDto;
        }
        return null;
    }
}
