using W2K.Common.Entities;
using W2K.Identity.Application.DTOs;
using W2K.Identity.Application.Enums;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application.Queries;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record GetOfficeUsersQuery(int Page, int PageSize, int? OfficeId, OfficeUsersSortColumn SortBy, bool SortDescending, bool CleanseSensitiveFields = false) : IRequest<PagedList<OfficeUserDto>>;
