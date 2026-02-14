using DFI.Common.Application.Dtos;
using MediatR;

namespace DFI.Common.Application.Queries.GlobalSearch;

/// <summary>
/// Query for global search across offices and users.
/// </summary>
/// <param name="SearchTerm">The search term to find offices and users.</param>
public record GlobalSearchQuery(string SearchTerm) : IStreamRequest<GlobalSearchResultDto>;
