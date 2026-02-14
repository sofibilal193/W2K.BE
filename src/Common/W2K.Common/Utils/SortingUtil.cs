using System.Linq.Expressions;

namespace DFI.Common.Utils;

/// <summary>
/// Utility class for applying sorting operations to collections and queries.
/// Provides generic helpers for both in-memory and database-backed ordering.
/// </summary>
public static class SortingUtil
{
    /// <summary>
    /// Generic helper to apply ordering to in-memory collections.
    /// Returns a read-only list with the specified ordering applied.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key to sort by.</typeparam>
    /// <param name="data">The collection to sort.</param>
    /// <param name="descending">True for descending order, false for ascending.</param>
    /// <param name="keySelector">A function to extract the sort key from each element.</param>
    /// <returns>A read-only list with the elements sorted according to the specified criteria.</returns>
    public static IReadOnlyList<T> ApplyOrderToCollection<T, TKey>(
        this IReadOnlyCollection<T> data,
        Func<T, TKey> keySelector,
        bool descending = false)
    {
        IOrderedEnumerable<T> ordered = descending
            ? data.OrderByDescending(keySelector)
            : data.OrderBy(keySelector);

        return ordered.ToList().AsReadOnly();
    }

    /// <summary>
    /// Generic helper to apply ordering to IQueryable (database queries).
    /// Leverages EF Core's query translation for efficient database-side sorting.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queryable.</typeparam>
    /// <typeparam name="TKey">The type of the key to sort by.</typeparam>
    /// <param name="query">The queryable to sort.</param>
    /// <param name="descending">True for descending order, false for ascending.</param>
    /// <param name="keySelector">An expression to extract the sort key from each element.</param>
    /// <returns>An ordered queryable with the sort criteria applied.</returns>
    public static IQueryable<T> OrderBy<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        bool descending = false)
    {
        return descending
            ? Queryable.OrderByDescending(query, keySelector)
            : Queryable.OrderBy(query, keySelector);
    }
}
