using AutoMapper;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class GetUserOfficeListQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<GetUserOfficeListQuery, IList<UserOfficeListItemDto>>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task<IList<UserOfficeListItemDto>> Handle(GetUserOfficeListQuery query, CancellationToken cancellationToken)
    {
        var officeUsers = await _data.OfficeUsers.AsNoTracking()
            .Include("Office.Addresses")
            .GetAsync(
                x => !x.IsDisabled && (_currentUser.OfficeType == OfficeType.SuperAdmin || x.UserId == _currentUser.UserId)
                        && !(x.Office!.Type == OfficeType.SuperAdmin || x.Office.Type == OfficeType.Lender),
                cancellationToken);

        return _mapper.Map<IList<UserOfficeListItemDto>>(officeUsers.DistinctBy(x => x.OfficeId));
    }
}
