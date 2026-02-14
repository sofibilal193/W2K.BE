namespace W2K.Common.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
#pragma warning disable S3925 // Update this implementation of 'ISerializable' to conform to the recommended serialization pattern.
// S3925 says to override GetDataObject to include any instance fields in the serialization process since they are
// not included automatically, however there are none declared here, so it's a false positive
public class DomainException : Exception
{
    public DomainException()
    { }

    public DomainException(string message)
        : base(message)
    { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
#pragma warning restore S3925 // Update this implementation of 'ISerializable' to conform to the recommended serialization pattern.
