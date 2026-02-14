using W2K.Config.Application.DTOs;
using W2K.Config.Repositories;
using AutoMapper;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Config.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetExternalConfigsQueryHandler(
    IConfigUnitOfWork data,
    IMapper mapper
) : IRequestHandler<GetExternalConfigsQuery, IList<ConfigDto>>
{
    private readonly IConfigUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<IList<ConfigDto>> Handle(GetExternalConfigsQuery query, CancellationToken cancellationToken)
    {
        var configs = await _data.Configs.AsNoTracking().GetAllAsync(x => !x.IsDisabled && !x.IsInternal, cancel: cancellationToken);

        var dto = _mapper.Map<List<ConfigDto>>(
            configs.OrderBy(x => x.Type).ThenBy(x => x.DisplayOrder));
        return dto;
    }
}
