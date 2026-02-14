#pragma warning disable IDE0046 // Use conditional expression for return
#pragma warning disable CA1308 // Normalize strings to uppercase

using System.Text.Json;
using System.Text.Json.Serialization;

namespace DFI.Common.Converters;

/// <summary>
/// Removes malicious characters from strings and trims whitespace when deserializing from JSON.
/// If string is empty or only contains whitespace, null is returned.
/// </summary>
public class BooleanJsonConverter : JsonConverter<bool>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(bool);
    }

    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Support native boolean tokens directly
        if (reader.TokenType == JsonTokenType.True)
        {
            return true;
        }
        if (reader.TokenType == JsonTokenType.False)
        {
            return false;
        }

        // Null -> default(bool) = false
        if (reader.TokenType == JsonTokenType.Null)
        {
            return false;
        }

        // Numeric tokens: treat 0 as false, any other number as true
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt64(out var l))
            {
                return l != 0;
            }
            if (reader.TryGetDouble(out var d))
            {
                return Math.Abs(d) > double.Epsilon;
            }
            return false;
        }

        // Strings (including those with surrounding whitespace)
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString()?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            return value.ToLowerInvariant() switch
            {
                "true" or "yes" or "y" or "1" => true,
                "false" or "no" or "n" or "0" => false,
                _ => false,
            };
        }

        // Any other token types default to false
        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

#pragma warning restore IDE0046 // Use conditional expression for return
#pragma warning restore CA1308 // Normalize strings to uppercase
