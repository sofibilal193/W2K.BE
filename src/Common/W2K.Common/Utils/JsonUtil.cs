using System.Text.Json;
using W2K.Common.Constants;

namespace W2K.Common.Utils;

public static class JsonUtil
{
    public static MemoryStream SerializeJsonIntoStream(object value)
    {
        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, value, value.GetType(), JsonOptions.IgnoreNullNotIndented);
        return stream;
    }

    public static T? TryParseTo<T>(this string s)
    {
        T? val = default;

        if (!string.IsNullOrEmpty(s))
        {
            try
            {
                val = JsonSerializer.Deserialize<T>(s, JsonOptions.CaseInsensitive);
            }
            catch
            {
                // ignore, default value will be returned
            }
        }

        return val;
    }

    public static T? ParseJsonFile<T>(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path is null or empty", nameof(path));
        }

        if (path.Contains("../", StringComparison.OrdinalIgnoreCase) || path.Contains(@"..\", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid file path");
        }

        // Normalize path and ensure it is within the application base directory to prevent traversal.
        var baseDir = Path.GetFullPath(AppContext.BaseDirectory);
        var fullPath = Path.GetFullPath(path);

        if (!fullPath.StartsWith(baseDir, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid file path");
        }

        if (!File.Exists(fullPath))
        {
            return default;
        }

        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static string Serialize<T>(this T o)
    {
        return JsonSerializer.Serialize<T>(o, JsonOptions.IgnoreNull);
    }

    public static Dictionary<string, object?>? ToDictionary<T>(this T o)
    {
        var json = o.Serialize();
        return string.IsNullOrEmpty(json)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, object?>>(json, JsonOptions.IgnoreNull);
    }

    // System.Text.Json based replacement (removed Newtonsoft.Json dependency)
    public static Dictionary<string, string>? ToKeyValue(this object metaToken)
    {
        if (metaToken is null)
        {
            return null;
        }

        if (metaToken is JsonDocument jd)
        {
            return FlattenElement(jd.RootElement);
        }

        var json = JsonSerializer.Serialize(metaToken, metaToken.GetType(), JsonOptions.CaseInsensitiveWithConverters);
        using var doc = JsonDocument.Parse(json);
        return FlattenElement(doc.RootElement);
    }

    private static Dictionary<string, string> FlattenElement(JsonElement root)
    {
        var dict = new Dictionary<string, string>();
        Traverse(root, "", dict);
        return dict;
    }

    private static void Traverse(JsonElement element, string path, Dictionary<string, string> dict)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                ProcessObject(element, path, dict);
                return;
            case JsonValueKind.Array:
                ProcessArray(element, path, dict);
                return;
            case JsonValueKind.String:
                ProcessString(element, path, dict);
                return;
            case JsonValueKind.Number:
                dict[path] = element.ToString();
                return;
            case JsonValueKind.True:
            case JsonValueKind.False:
                dict[path] = element.GetBoolean().ToString(System.Globalization.CultureInfo.InvariantCulture);
                return;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                return;
        }
    }

    private static void ProcessObject(JsonElement element, string path, Dictionary<string, string> dict)
    {
        foreach (var prop in element.EnumerateObject())
        {
            var childPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
            Traverse(prop.Value, childPath, dict);
        }
    }

    private static void ProcessArray(JsonElement element, string path, Dictionary<string, string> dict)
    {
        int i = 0;
        foreach (var item in element.EnumerateArray())
        {
            var childPath = string.IsNullOrEmpty(path) ? $"[{i}]" : $"{path}[{i}]";
            Traverse(item, childPath, dict);
            i++;
        }
    }

    private static void ProcessString(JsonElement element, string path, Dictionary<string, string> dict)
    {
        var s = element.GetString();
        if (!string.IsNullOrEmpty(s)
            && DateTime.TryParse(
                s,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out var dt))
        {
            dict[path] = dt.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (s is not null)
        {
            dict[path] = s;
        }
    }
}
