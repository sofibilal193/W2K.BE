#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DFI.Common.Application.ModelBinders;

public class JsonModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        await Task.Yield(); // Simulate asynchronous operation

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return;
        }

        try
        {
            // Deserialize the JSON payload into the expected model type
            var result = JsonSerializer.Deserialize(value, bindingContext.ModelType, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (JsonException ex)
        {
            _ = bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex, bindingContext.ModelMetadata);
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances
