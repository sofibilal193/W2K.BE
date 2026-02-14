using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace W2K.Common.Application.Filters;

// Suppress Sonar warning: S2325 - Methods and properties that don't access instance data should be static
#pragma warning disable S2325

public class AddSessionIdHeaderProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Session-Id",
            Kind = OpenApiParameterKind.Header,
            Schema = new JsonSchema { Type = JsonObjectType.String },
            IsRequired = false,
            Description = "An optional unique identifier for the transaction or user's session to help with troubleshooting.",
        });
        return true;
    }
}

#pragma warning restore S2325
