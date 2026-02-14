#pragma warning disable IDE0046 // Use conditional expression for return

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace W2K.Common.Converters;

/// <summary>
/// Generic converter that converts a JSON string to a nullable value type.
/// If string is empty or only contains whitespace, null is returned.
/// Supports DateOnly, decimal, int, long, double, and float.
/// For bool conversion, use BooleanJsonConverter instead.
/// </summary>
/// <typeparam name="T">The nullable value type to convert to.</typeparam>
/// <param name="formatProvider">Culture-specific formatting provider. Defaults to en-US for DateOnly, InvariantCulture for numerics.</param>
public class NullableStringConverter<T>(IFormatProvider? formatProvider) : JsonConverter<T?> where T : struct
{
    private static readonly Func<string, IFormatProvider?, T?>? _parser = InitializeParser();
    private readonly IFormatProvider? _formatProvider = formatProvider;

    public NullableStringConverter() : this(null) { }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(T?);
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString()?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return _parser?.Invoke(value, _formatProvider);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString());
        }
        else
        {
            writer.WriteNullValue();
        }
    }

    private static Func<string, IFormatProvider?, T?>? InitializeParser()
    {
        var targetType = typeof(T);

        if (targetType == typeof(DateOnly))
        {
            return (value, provider) =>
            {
                provider ??= new CultureInfo("en-US");
                if (DateOnly.TryParse(value, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        if (targetType == typeof(decimal))
        {
            return (value, provider) =>
            {
                provider ??= CultureInfo.InvariantCulture;
                if (decimal.TryParse(value, NumberStyles.Number, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        if (targetType == typeof(int))
        {
            return (value, provider) =>
            {
                provider ??= CultureInfo.InvariantCulture;
                if (int.TryParse(value, NumberStyles.Integer, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        if (targetType == typeof(long))
        {
            return (value, provider) =>
            {
                provider ??= CultureInfo.InvariantCulture;
                if (long.TryParse(value, NumberStyles.Integer, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        if (targetType == typeof(double))
        {
            return (value, provider) =>
            {
                provider ??= CultureInfo.InvariantCulture;
                if (double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        if (targetType == typeof(float))
        {
            return (value, provider) =>
            {
                provider ??= CultureInfo.InvariantCulture;
                if (float.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, provider, out var result))
                {
                    return (T)(object)result;
                }
                return null;
            };
        }

        return null;
    }
}

#pragma warning restore IDE0046 // Use conditional expression for return
