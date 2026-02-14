using Microsoft.Extensions.Hosting;

namespace DFI.Common.Application.Extensions;

public static class IHostEnvironmentExtensions
{
    public static bool IsTest(this IHostEnvironment env)
    {
        return env.IsEnvironment("Test");
    }
}
