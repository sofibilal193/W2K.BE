using System.Text.Json.Serialization;

namespace DFI.Common.Models;

/// <summary>
/// Represents a base response from a service.
/// </summary>
public abstract record Response
{
    /// <summary>
    /// Indicates whether the response resulted in an error.
    /// </summary>
    public bool IsError { get; private set; }

    /// <summary>
    /// Error code if the response failed; otherwise null.
    /// </summary>
    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Error message if the response failed; otherwise null.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    protected Response()
    {
    }

    protected Response(bool isError, string? errorCode, string? errorMessage)
    {
        IsError = isError;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public void SetError(bool isError, string? errorCode, string? errorMessage)
    {
        IsError = isError;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
