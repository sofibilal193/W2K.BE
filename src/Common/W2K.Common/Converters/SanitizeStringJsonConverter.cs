using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DFI.Common.Constants;

namespace DFI.Common.Converters;

/// <summary>
/// Removes malicious characters from strings and trims whitespace when deserializing from JSON.
/// If string is empty or only contains whitespace, null is returned.
/// </summary>
public class SanitizeStringJsonConverter : JsonConverter<string>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()?.Trim();
        return string.IsNullOrEmpty(value) ? null : Regex.Replace(value, RegularExpressions.SanitizeJson, string.Empty, RegexOptions.NonBacktracking);
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}
