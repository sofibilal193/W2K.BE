namespace DFI.Common.Attributes;

/// <summary>
/// Attribute to specify custom CSV header name for a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CsvFieldHeaderAttribute : Attribute
{
    /// <summary>
    /// Gets the custom header name for the CSV field.
    /// </summary>
    public string HeaderName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvFieldAttribute"/> class.
    /// </summary>
    /// <param name="headerName">The custom header name for the CSV field.</param>
    public CsvFieldHeaderAttribute(string headerName)
    {
        HeaderName = headerName;
    }
}
