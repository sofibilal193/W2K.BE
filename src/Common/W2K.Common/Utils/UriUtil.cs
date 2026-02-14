namespace DFI.Common.Utils;

public static class UriUtil
{
    public static Uri[] GetUris(this string value)
    {
        List<Uri> endpoints = [];
        if (value.Contains(';', StringComparison.OrdinalIgnoreCase))
        {
            var uris = value.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var uri in uris)
            {
                endpoints.Add(new Uri(uri));
            }
        }
        else
        {
            endpoints.Add(new Uri(value));
        }
        return [.. endpoints];
    }
}
