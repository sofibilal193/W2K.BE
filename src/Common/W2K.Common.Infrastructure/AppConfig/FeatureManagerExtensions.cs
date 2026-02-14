using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace W2K.Common.Infrastructure.AppConfig;

public static class FeatureManagerExtensions
{
    /// <summary>
    /// Checks whether a given feature is enabled for an office.
    /// </summary>
    /// <param name="featureManager">Feature Manager instance.</param>
    /// <param name="feature">The name of the feature to check.</param>
    /// <param name="officeId">Id of office to check if feature is enabled for.</param>
    /// <returns>True if the feature is enabled for office, otherwise false.</returns>
    public static Task<bool> IsEnabledForOfficeAsync(this IFeatureManager featureManager, string feature, int officeId)
    {
        var context = new TargetingContext
        {
            Groups = [$"OfficeId:{officeId}"]
        };
        return featureManager.IsEnabledAsync(feature, context);
    }
}
