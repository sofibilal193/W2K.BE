using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DFI.Common.Application.Validations;

public class ApiValidationProblemDetails(ModelStateDictionary modelState) : ValidationProblemDetails(modelState)
{
    public string? TraceId { get; set; }

    public Dictionary<string, string[]> ErrorCodes { get; private set; } = [];
}
