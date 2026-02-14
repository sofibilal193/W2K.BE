using NJsonSchema.Generation;

namespace DFI.Common.Application.Filters;

// Suppress Sonar warning: S2325 - Methods and properties that don't access instance data should be static
#pragma warning disable S2325

public class ApiSchemaNameGenerator : ISchemaNameGenerator
{
    public string Generate(Type type)
    {
        // Remove "Dto" from class names
        if (type.IsGenericType)
        {
            var genericType = type.Name.Split('`')[0]?.Replace("Dto", "", StringComparison.OrdinalIgnoreCase);
            var arguments = string.Join(", ", type.GenericTypeArguments.Select(x => x.Name.Replace("Dto", "", StringComparison.OrdinalIgnoreCase)));
            return $"{genericType}<{arguments}>"; // NSwag will convert this to GenericTypeOfArguments (e.g. PagedListOfOrg)
        }
        else
        {
            return type.Name.Replace("Dto", "", StringComparison.OrdinalIgnoreCase);
        }
    }
}

#pragma warning restore S2325
