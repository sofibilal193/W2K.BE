using AutoMapper;
using W2K.Common.Enums;
using W2K.Common.Identity;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handler for searching offices by ID or name.
/// </summary>
public class SearchOfficesQueryHandler(IIdentityUnitOfWork data, IMapper mapper, ICurrentUser currentUser) : IRequestHandler<SearchOfficesQuery, IList<OfficeSearchResultDto>>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ICurrentUser _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

    public async Task<IList<OfficeSearchResultDto>> Handle(SearchOfficesQuery query, CancellationToken cancellationToken)
    {
        var isSuperAdmin = _currentUser.OfficeType == OfficeType.SuperAdmin;
        var isInteger = int.TryParse(query.SearchTerm, out var officeId);
        var searchTerm = $"%{query.SearchTerm?.Trim().ToLowerInvariant()}%";

        var offices = await _data.Offices
            .AsNoTracking()
            .GetAsync(
                x => !x.IsDisabled
                        && x.Type != OfficeType.SuperAdmin
                        && x.Type != OfficeType.Lender
                        && (isInteger ? x.Id == officeId : EF.Functions.Like(x.Name, searchTerm))
                        && (isSuperAdmin || (_currentUser.OfficeIds != null && _currentUser.OfficeIds.Contains(x.Id))),
                cancellationToken);

        var officeResults = _mapper.Map<List<OfficeSearchResultDto>>(offices);

        return officeResults;
    }
}
