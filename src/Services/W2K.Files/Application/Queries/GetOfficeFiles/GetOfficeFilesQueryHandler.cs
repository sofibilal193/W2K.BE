using AutoMapper;
using W2K.Files.Application.DTOs;
using W2K.Files.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Files.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetOfficeFilesQueryHandler(IFilesUnitOfWork data, IMapper mapper) : IRequestHandler<GetOfficeFilesQuery, IEnumerable<OfficeFilesDto>>
{
    private readonly IFilesUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<OfficeFilesDto>> Handle(GetOfficeFilesQuery query, CancellationToken cancellationToken)
    {
        var files = await _data.Files.Include(x => x.Tags).GetAsync(x => x.OfficeId == query.OfficeId, cancellationToken);

        return _mapper.Map<List<OfficeFilesDto>>(files);
    }
}
