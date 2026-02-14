#pragma warning disable AIK_csharp_injection_XPathInjection // Xpath injection attack could lead to information extraction
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace W2K.Common.Utils;

public static class LogUtil
{
    /// <summary>
    /// Masks a field value within JSON, XML, or key-value pair content.
    /// </summary>
    /// <param name="content">String content containing field to mask.</param>
    /// <param name="fieldPath">Path to field to mask (JSON DOM Path, XPath, or KeyValue key).</param>
    /// <param name="mask">String to replace actual value with.</param>
    /// <param name="contentType">Content type of content.</param>
    /// <returns>Content with specified field value masked.</returns>
    public static string MaskFields(string content, IEnumerable<string> fieldPaths, string mask, LogContentType contentType)
    {
        if (contentType == LogContentType.JSON)
        {
            return MaskJsonContent(content, fieldPaths, mask);
        }
        else if (contentType == LogContentType.XML)
        {
            return MaskXmlContent(content, fieldPaths, mask);
        }
        else if (contentType == LogContentType.KeyValuePairs)
        {
            return MaskKeyValuePairs(content, fieldPaths, mask);
        }
        return content;
    }

    private static string MaskJsonContent(string content, IEnumerable<string> fieldPaths, string mask)
    {
        var json = JToken.Parse(content);
        if (json is null)
        {
            return content;
        }
        foreach (var path in fieldPaths)
        {
            var tokens = json.SelectTokens(path);
            foreach (var token in tokens)
            {
                if (token.Parent is JProperty property && !string.IsNullOrEmpty(property.Value.ToString()))
                {
                    property.Value = mask;
                }
            }
        }
        return json.ToString();
    }

    private static string MaskXmlContent(string content, IEnumerable<string> fieldPaths, string mask)
    {
        var xdoc = XDocument.Parse(content);

        foreach (var path in fieldPaths)
        {
            // Strict XPath validation: allow only simple element/attribute names, no functions or predicates
            // Only allow paths like: /root/element, //element, /root/element/@attr, etc.
            // Pattern allows: /element or //element followed by more /element and optionally /@attribute
            var xpathPattern = @"^(/)+([\w:]+)(/[\w:]+)*(/@[\w:]+)?$";
            if (string.IsNullOrWhiteSpace(path) || !System.Text.RegularExpressions.Regex.IsMatch(path, xpathPattern))
            {
                // Skip potentially dangerous or malformed XPath expressions
                continue;
            }
            // Defensive: Only allow simple, safe XPath expressions (elements and attributes, no predicates/functions)
            try
            {
                var pathParts = path.Split("/@");
                var elements = path.EndsWith("/@*", StringComparison.Ordinal) || path.Contains("/@", StringComparison.Ordinal)
                    ? xdoc.XPathSelectElements(path[..path.LastIndexOf("/@", StringComparison.Ordinal)]).Where(x => x.Attribute(pathParts[^1])?.Value is not null)
                    : xdoc.XPathSelectElements(path).Where(x => !string.IsNullOrEmpty(x.Value));

                foreach (var element in elements)
                {
                    if (path.Contains("/@", StringComparison.Ordinal))
                    {
                        var attrName = pathParts[^1];
                        element.Attribute(attrName)!.Value = mask;
                    }
                    else
                    {
                        element.Value = mask;
                    }
                }
            }
            catch (XPathException ex) when (ex.Message.StartsWith("Namespace prefix", StringComparison.OrdinalIgnoreCase))
            {
                // ignore (this can occur if path contains a namespace not defined in XML)
            }
        }
        return xdoc.ToString();
    }

    private static string MaskKeyValuePairs(string content, IEnumerable<string> fieldPaths, string mask)
    {
        var maskedPairs = new List<string>();
        var pairs = content.Split('&');
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=');
            maskedPairs.Add(parts.Length > 1 && fieldPaths.Contains(parts[0]) && !string.IsNullOrEmpty(parts[1])
                ? $"{parts[0]}={mask}"
                : pair);
        }
        return string.Join('&', maskedPairs);
    }
}

public enum LogContentType
{
    JSON,
    XML,
    KeyValuePairs
}

#pragma warning restore AIK_csharp_injection_XPathInjection // Xpath injection attack could lead to information extraction
