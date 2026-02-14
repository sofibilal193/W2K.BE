namespace W2K.Common.Application.Queries.GlobalSearch;

/// <summary>
/// Defines the type of search result returned from global search.
/// </summary>
public enum SearchResultType
{
    /// <summary>
    /// Search result is an Office.
    /// </summary>
    Office = 0,

    /// <summary>
    /// Search result is a User.
    /// </summary>
    User = 1,

    /// <summary>
    /// Search result is a Loan Application.
    /// </summary>
    LoanApp = 2,

    /// <summary>
    /// Search result is a Loan.
    /// </summary>
    Loan = 3
}
