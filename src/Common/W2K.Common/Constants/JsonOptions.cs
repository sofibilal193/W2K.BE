using System.Text.Json;
using System.Text.Json.Serialization;
using W2K.Common.Converters;

namespace W2K.Common.Constants;

/// <summary>
/// Common JSON options
/// </summary>
public static class JsonOptions
{

    public static readonly JsonSerializerOptions IgnoreNullNotIndented = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static readonly JsonSerializerOptions CaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static readonly JsonSerializerOptions IgnoreNull = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static JsonSerializerOptions CaseInsensitiveWithConverters => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new SanitizeStringJsonConverter(),
            new BooleanJsonConverter()
        }
    };

}
