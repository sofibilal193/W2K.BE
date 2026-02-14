namespace W2K.Common.Attributes;

/// <summary>
/// Attribute to specify custom CSV header name for a property.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CsvFieldAttribute"/> class.
/// </remarks>
/// <param name="headerName">The custom header name for the CSV field.</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class CsvFieldHeaderAttribute(string headerName) : Attribute
{
    /// <summary>
    /// Gets the custom header name for the CSV field.
    /// </summary>
    public string HeaderName { get; } = headerName;
}
