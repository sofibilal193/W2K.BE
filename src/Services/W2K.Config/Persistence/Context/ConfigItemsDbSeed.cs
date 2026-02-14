using System.Text.Json;
using W2K.Common.Config;
using W2K.Common.Extensions;
using W2K.Common.Identity;
using W2K.Common.Utils;
using W2K.Config.Entities;
using W2K.Config.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace W2K.Config.Persistence.Context;

public static class ConfigItemsDbSeed
{
    public static async Task SeedAsync(IConfigUnitOfWork data, ILogger logger)
    {
        var pipeline = CreatePipeline(logger, nameof(ConfigItemsDbSeed));
        await pipeline.ExecuteAsync(async _ =>
            {
                await SeedConfigItemsAsync(data);
                await SeedOfficeConfigFieldsAsync(data);
            });
    }

    private static ResiliencePipeline CreatePipeline(ILogger logger, string prefix, int retries = 3, int delaySeconds = 5)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<SqlException>(),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                MaxRetryAttempts = retries,
                Delay = TimeSpan.FromSeconds(delaySeconds),
                OnRetry = x =>
                {
                    logger.LogWarning(
                        x.Outcome.Exception,
                        "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix,
                        x.Outcome.Exception?.GetType().Name,
                        x.Outcome.Exception?.Message,
                        x.AttemptNumber,
                        retries);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    private static async Task SeedConfigItemsAsync(IConfigUnitOfWork data)
    {
        bool saveChanges = false;
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "Config", "ConfigItems.json");
        if (!string.IsNullOrEmpty(path))
        {
            var configItems = ParseConfigItemsFromJson(path);
            if (configItems is not null)
            {
                foreach (var item in configItems)
                {
                    var exists = await data.Configs.AnyAsync(x => x.Name == item.Name && x.Type == item.Type);
                    if (!exists)
                    {
                        saveChanges = true;
                        data.Configs.Add(item);
                    }
                }
            }
        }
        if (saveChanges)
        {
            _ = await data.SaveEntitiesAsync();
        }
    }

    private static List<Entities.Config> ParseConfigItemsFromJson(string path)
    {
        var configs = new List<Entities.Config>();
        var elements = JsonUtil.ParseJsonFile<JsonElement>(path);
        foreach (var element in elements.EnumerateArray())
        {
            var type = element.GetPropertyValue<string>("Type");
            var name = element.GetPropertyValue<string>("Name");
            var description = element.GetPropertyValue<string>("Description");
            var value = element.GetPropertyValue<string>("Value");
            var displayOrder = element.GetPropertyValue<short>("DisplayOrder");
            var isInternal = element.GetPropertyValue<bool>("IsInternal");
            var isEncrypted = element.GetPropertyValue<bool>("IsEncrypted");

            var info = new Entities.Config.ConfigItemInfo(type, name, description, value, displayOrder, isInternal, isEncrypted);
            configs.Add(new Entities.Config(info));
        }
        return configs;
    }

    private static async Task SeedOfficeConfigFieldsAsync(IConfigUnitOfWork data)
    {
        bool saveChanges = false;
        var path = Path.Combine(AppContext.BaseDirectory, "Seed", "Config", "OfficeConfigFields.json");
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            var fields = ParseOfficeConfigFieldsFromJson(path);
            if (fields is not null)
            {
                foreach (var field in fields)
                {
                    var exists = await data.OfficeConfigFields.AnyAsync(x => x.Name == field.Name && x.OfficeType == field.OfficeType);
                    if (!exists)
                    {
                        saveChanges = true;
                        data.OfficeConfigFields.Add(field);
                    }
                }
            }
        }
        if (saveChanges)
        {
            _ = await data.SaveEntitiesAsync();
        }
    }

    private static List<OfficeConfigField> ParseOfficeConfigFieldsFromJson(string path)
    {
        var fields = new List<OfficeConfigField>();
        var elements = JsonUtil.ParseJsonFile<JsonElement>(path);
        foreach (var element in elements.EnumerateArray())
        {
            var officeTypeString = element.GetPropertyValue<string>("OfficeType");
            OfficeType? officeType = string.IsNullOrEmpty(officeTypeString) ? null : Enum.Parse<OfficeType>(officeTypeString);
            var category = element.GetPropertyValue<string>("Category");
            var fieldTypeString = element.GetPropertyValue<string>("FieldType") ?? nameof(FieldType.Text);
            var fieldType = Enum.Parse<FieldType>(fieldTypeString);
            var name = element.GetPropertyValue<string>("Name");
            var description = element.GetPropertyValue<string>("Description");
            var defaultValue = element.GetPropertyValue<string>("DefaultValue");
            var displayOrder = element.GetPropertyValue<short>("DisplayOrder");
            var minValue = element.GetPropertyValue<decimal?>("MinValue");
            var maxValue = element.GetPropertyValue<decimal?>("MaxValue");
            var regexValidator = element.GetPropertyValue<string>("RegexValidator");
            var isInternal = element.GetPropertyValue<bool>("IsInternal");
            var isEncrypted = element.GetPropertyValue<bool>("IsEncrypted");

            var info = new OfficeConfigField.OfficeConfigFieldInfo(
                officeType,
                category,
                fieldType,
                name,
                description,
                defaultValue,
                displayOrder,
                minValue,
                maxValue,
                regexValidator,
                isInternal,
                isEncrypted);
            fields.Add(new OfficeConfigField(info));
        }
        return fields;
    }
}
