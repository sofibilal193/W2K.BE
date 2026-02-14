using W2K.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace W2K.Common.Persistence.Extensions;

public static class PagedListExtensions
{
    /// <summary>
    /// Returns a subset of items from an IQueryable for a give page and page size.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source.</typeparam>
    /// <param name="source">An System.Linq.IQueryable&lt;<typeparamref name="TSource"/>&gt; to get the
    /// paginated subset from (typically a Microsoft.EntityFrameworkCore.DbSet&lt;<typeparamref name="TSource"/>&gt;).</param>
    /// <param name="page">Page number to get results for.</param>
    /// <param name="pageSize">Maximum number of items to return.</param>
    /// <returns>A PagedList containing the items for the specified page as well as pagination metadata.</returns>
    public static PagedList<TSource> ToPagedList<TSource>(this IQueryable<TSource> source, int page, int pageSize)
    {
        return new PagedList<TSource>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = source.Count(),
            Items = source.Skip((page * pageSize) - pageSize).Take(pageSize).ToList()
        };
    }

    /// <summary>
    /// Asynchronously retrieves and returns a subset of items from an IQueryable for a give page and page size.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the source.</typeparam>
    /// <param name="source">An System.Linq.IQueryable&lt;<typeparamref name="TSource"/>&gt; to get the
    /// paginated subset from (typically a Microsoft.EntityFrameworkCore.DbSet&lt;<typeparamref name="TSource"/>&gt;).</param>
    /// <param name="page">Page number to get results for.</param>
    /// <param name="pageSize">Maximum number of items to return.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A PagedList containing the items for the specified page as well as pagination metadata.</returns>
    public static async Task<PagedList<TSource>> ToPagedListAsync<TSource>(
        this IQueryable<TSource> source,
        int page,
        int pageSize,
        CancellationToken cancel = default
    )
    {
        return new PagedList<TSource>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = await source.CountAsync(cancel),
            Items = await source.Skip((page * pageSize) - pageSize).Take(pageSize).ToListAsync(cancel)
        };
    }
}
