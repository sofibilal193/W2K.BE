using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Enums;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record GetOfficesForSuperAdminQuery(int Page, int PageSize, string? Search, OfficeSortColumn SortBy, bool SortDescending)
              : IRequest<PagedList<OfficeListItemDto>>;

