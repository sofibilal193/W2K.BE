namespace DFI.Common.Models;

/// <summary>
/// Model containing information about a loan application.
/// </summary>
public readonly record struct LoanAppInfo
{
    /// <summary>
    /// The unique identifier for the loan application.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The unique identifier for the office.
    /// </summary>
    public int OfficeId { get; init; }

    /// <summary>
    /// The Id of the parent (original) application.
    /// </summary>
    public int? ParentAppId { get; init; }

    /// <summary>
    /// The name of the service associated with the loan.
    /// </summary>
    public string ServiceName { get; init; }

    /// <summary>
    /// The date the service will be provided.
    /// </summary>
    public DateOnly? ServiceDate { get; init; }

    /// <summary>
    /// The requested loan amount.
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// The primary borrower information.
    /// </summary>
    public BorrowerInfo Borrower { get; init; }

    /// <summary>
    /// The individual receiving service (if different than Borrower).
    /// </summary>
    public BorrowerInfo? ServiceReceiver { get; init; }

    /// <summary>
    /// Indicates if the Service Receiver is the same as the Borrower.
    /// </summary>
    public bool IsServiceReceiverSameAsBorrower => ServiceReceiver is null || (Borrower.FirstName == ServiceReceiver.Value.FirstName && Borrower.LastName == ServiceReceiver.Value.LastName);
}
