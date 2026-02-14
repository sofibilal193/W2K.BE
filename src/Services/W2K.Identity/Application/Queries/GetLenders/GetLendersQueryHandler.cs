using AutoMapper;
using W2K.Common.Application;
using W2K.Common.Identity;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetLendersQueryHandler(IIdentityUnitOfWork data, IMapper mapper) : IRequestHandler<GetLendersQuery, List<LenderDto>>
{
    private readonly IIdentityUnitOfWork _data = data;
    private readonly IMapper _mapper = mapper;

    public async Task<List<LenderDto>> Handle(GetLendersQuery query, CancellationToken cancellationToken)
    {
        var lenders = await _data.Offices.AsNoTracking().GetAsync(x => x.Type == OfficeType.Lender && !x.IsDisabled, cancellationToken);

        return _mapper.Map<List<LenderDto>>(lenders);
    }
}
