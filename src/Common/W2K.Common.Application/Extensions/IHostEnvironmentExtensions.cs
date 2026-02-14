using Microsoft.Extensions.Hosting;

namespace W2K.Common.Application.Extensions;

public static class IHostEnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment env)
    {
        return env.IsEnvironment("Test");
    }
}
