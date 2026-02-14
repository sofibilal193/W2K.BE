using System.Reflection;
using W2K.Common.Attributes;

namespace W2K.Common.Utils;

/// <summary>
/// Utility class for CSV file operations.
/// </summary>
public static class CsvUtil
{
    /// <summary>
    /// Writes a collection of items to a CSV byte array.
    /// </summary>
    /// <typeparam name="T">The type of items to write.</typeparam>
    /// <param name="items">The collection of items to write.</param>
    /// <returns>A byte array containing the CSV data</returns>
    public static byte[] Write<T>(IReadOnlyCollection<T> items)
    {
        if (items.Count == 0)
        {
            return Array.Empty<byte>();
        }

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);

        // get properties of T type
        var properties = typeof(T).GetProperties();

        // write header from attributes (if present) or property names by default
        writer.WriteLine(string.Join(',', properties.Select(x => x.GetCustomAttribute<CsvFieldHeaderAttribute>()?.HeaderName ?? x.Name)));

        // write items to csv
        foreach (var item in items)
        {
            var values = properties.Select(x =>
                {
                    return x.GetValue(item)?.ToString() ?? string.Empty;

                });
            writer.WriteLine(string.Join(',', values));
        }

        writer.Flush();
        stream.Position = 0;
        return stream.ToArray();
    }
}
