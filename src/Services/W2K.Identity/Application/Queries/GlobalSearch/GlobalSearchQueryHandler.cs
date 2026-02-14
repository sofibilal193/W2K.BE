using W2K.Common.Application.Dtos;
using W2K.Common.Application.Queries.GlobalSearch;
using W2K.Identity.Repositories;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Identity.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Handler for searching offices and users.
/// If OfficeIds is null, searches all offices and users (SuperAdmin access).
/// Otherwise, searches only within the specified OfficeIds.
/// </summary>
public class GlobalSearchQueryHandler(
    IIdentityUnitOfWork data) : IStreamRequestHandler<GlobalSearchQuery, GlobalSearchResultDto>
{
    private readonly IIdentityUnitOfWork _data = data ?? throw new ArgumentNullException(nameof(data));

    public async IAsyncEnumerable<GlobalSearchResultDto> Handle(GlobalSearchQuery request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var isInteger = int.TryParse(request.SearchTerm, out var id);
        var searchTerm = $"%{request.SearchTerm?.Trim().ToLowerInvariant()}%";

        // Search Offices and yield results as they are found
        await foreach (var office in _data.Offices.SearchOfficesForGlobalSearchAsync(id, searchTerm, isInteger).WithCancellation(cancellationToken))
        {
            yield return new GlobalSearchResultDto(
                SearchResultType.Office,
                office.Id,
                office.Name);
        }

        // Search Users and yield results as they are found
        await foreach (var user in _data.Users.SearchUsersForGlobalSearchAsync(id, searchTerm, isInteger).WithCancellation(cancellationToken))
        {
            // Map user to multiple results (one per office)
            foreach (var officeUser in user.Offices.Where(x => !x.IsDisabled && x.Office is not null))
            {
                yield return new GlobalSearchResultDto(
                    SearchResultType.User,
                    officeUser.OfficeId,
                    officeUser.Office?.Name,
                    user.Id,
                    user.FullName);
            }
        }
    }
}
