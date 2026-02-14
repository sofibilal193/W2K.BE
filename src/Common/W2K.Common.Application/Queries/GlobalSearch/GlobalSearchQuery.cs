using W2K.Common.Application.Dtos;
using MediatR;

namespace W2K.Common.Application.Queries.GlobalSearch;

/// <summary>
/// Query for global search across offices and users.
/// </summary>
/// <param name="SearchTerm">The search term to find offices and users.</param>
public record GlobalSearchQuery(string SearchTerm) : IStreamRequest<GlobalSearchResultDto>;
