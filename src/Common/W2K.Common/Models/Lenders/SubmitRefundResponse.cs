#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace DFI.Common.Models.Lenders;
#pragma warning restore IDE0130 // Namespace does not match folder structure


/// <summary>
/// Represents the response to a refund submission.
/// </summary>
public record SubmitRefundResponse
{
    public bool IsError { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
 
    public bool Success { get; set; }

    public void SetError(bool isError, string? errorCode, string? errorMessage)
    {
        IsError = isError;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
