using System.Text.Json;
using W2K.Common.Constants;

namespace W2K.Common.Extensions;

public static class JsonExtensions
{
    public static TValue? GetPropertyValue<TValue>(this JsonElement element, string propertyName)
    {
        var property = element.EnumerateObject()
            .FirstOrDefault(x => x.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        return property.Equals(default(JsonProperty))
            ? default
            : JsonSerializer.Deserialize<TValue>(property.Value.GetRawText(), JsonOptions.CaseInsensitiveWithConverters);
    }

    public static TValue? GetPropertyValue<TValue>(this JsonDocument document, string propertyName)
    {
        return document.RootElement.GetPropertyValue<TValue>(propertyName);
    }
}
