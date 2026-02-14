using DFI.Common.Persistence.Settings;
using Microsoft.Extensions.Configuration;

namespace DFI.Common.Persistence.Extensions;

public static class ConfigurationExtensions
{
    public static PersistenceSettings GetPersistenceSettings(this IConfiguration configuration, string applicationName)
    {
        return configuration.GetSection(applicationName).GetSection("PersistenceSettings").Get<PersistenceSettings>() ?? new();
    }

    public static string? GetConnectionString(this IConfiguration configuration, string applicationName, string connectionStringName)
    {
        return configuration.GetSection(applicationName).GetSection("PersistenceSettings").GetValue<string>(connectionStringName);
    }
}
