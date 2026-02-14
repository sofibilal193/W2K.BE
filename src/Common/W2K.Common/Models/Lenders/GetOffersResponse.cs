using System.Text.Json.Serialization;

namespace DFI.Common.Models.Lenders;

/// <summary>
/// Represents the response to retrieval of loan offers from a lender.
/// </summary>
public record GetOffersResponse : Response
{
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

    public GetOffersResponse()
    {
    }

    [JsonConstructor]
    public GetOffersResponse(
        string? lenderAppId,
        string? lenderAppUrl,
        DecisionInfo? decision,
        bool isError = false,
        string? errorCode = null,
        string? errorMessage = null
    ) : base(isError, errorCode, errorMessage)
    {
        LenderAppId = lenderAppId;
        LenderAppUrl = lenderAppUrl;
        Decision = decision;
    }

    public void SetDecision(DecisionInfo decision)
    {
        Decision = decision;
    }
}
