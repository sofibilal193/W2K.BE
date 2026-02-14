using DFI.Common.Application.Queries.GlobalSearch;

namespace DFI.Common.Application.Dtos;

/// <summary>
/// DTO representing a global search result item.
/// </summary>
public readonly record struct GlobalSearchResultDto(
    SearchResultType SearchResultType,
    int OfficeId,
    string? OfficeName,
    int? SearchResultId = null,
    string? SearchResultName = null);
