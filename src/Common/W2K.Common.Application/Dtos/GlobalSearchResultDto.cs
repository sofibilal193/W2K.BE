using W2K.Common.Application.Queries.GlobalSearch;

namespace W2K.Common.Application.Dtos;

/// <summary>
/// DTO representing a global search result item.
/// </summary>
public readonly record struct GlobalSearchResultDto(
    SearchResultType SearchResultType,
    int OfficeId,
    string? OfficeName,
    int? SearchResultId = null,
    string? SearchResultName = null);
