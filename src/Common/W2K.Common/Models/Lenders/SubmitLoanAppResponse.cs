
using System.Text.Json.Serialization;
namespace DFI.Common.Models.Lenders;

/// <summary>
/// Represents the response to a lender application submission.
/// </summary>
public record SubmitLoanAppResponse : Response
{
    /// <summary>
    /// Id of the lender to which the application was submitted.
    /// </summary>
    public int LenderId { get; private set; }

    /// <summary>
    /// The name of the lender to which the application was submitted.
    /// </summary>
    public string LenderName { get; private set; } = string.Empty;

    /// <summary>
    /// The identifier assigned to the application by the lender, if available.
    /// </summary>
    public string? LenderAppId { get; init; }

    /// <summary>
    /// The URL to access the application on the lender's platform, if available.
    /// </summary>
    public string? LenderAppUrl { get; init; }

    /// <summary>
    /// Decision information returned by the lender, if available.
    /// </summary>
    public DecisionInfo? Decision { get; private set; }

    public SubmitLoanAppResponse() : base()
    {
    }

    [JsonConstructor]
    public SubmitLoanAppResponse(
        int lenderId,
        string lenderName,
        string? lenderAppId,
        string? lenderAppUrl,
        DecisionInfo? decision,
        bool isError = false,
        string? errorCode = null,
        string? errorMessage = null
    ) : base(isError, errorCode, errorMessage)
    {
        LenderId = lenderId;
        LenderName = lenderName ?? string.Empty;
        LenderAppId = lenderAppId;
        LenderAppUrl = lenderAppUrl;
        Decision = decision;
    }

    public void SetDecision(DecisionInfo decision)
    {
        Decision = decision;
    }

    public void SetLenderDetails(int lenderId, string lenderName)
    {
        LenderId = lenderId;
        LenderName = lenderName;
    }
}
