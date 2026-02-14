using System.Text.Json.Serialization;
using W2K.Common.Models;
using MediatR;

namespace W2K.Common.Application.Commands.Loans;

public class UpsertLoanAppDecisionCommand : IRequest<int?>
{
    /// <summary>
    /// The ID of the lender.
    /// </summary>
    [JsonIgnore]
    public int LenderId { get; private set; }

    /// <summary>
    /// The ID of loan application in lender's system.
    /// </summary>
    public string LenderAppId { get; init; }

    public DecisionInfo Decision { get; init; }

    public UpsertLoanAppDecisionCommand(string lenderAppId, DecisionInfo decision)
    {
        LenderAppId = lenderAppId;
        Decision = decision;
    }

    public void SetLenderId(int lenderId)
    {
        LenderId = lenderId;
    }
}
