using System.Net;

namespace DFI.Common.Utils;

/// <summary>
/// Operating system utility methods.
/// </summary>
public static class OSUtil
{
    /// <summary>
    /// Gets the host name of the current machine.
    /// </summary>
    /// <returns>The host name of the machine.</returns>
    public static string GetHostName()
    {
        return Dns.GetHostName().ToLowerInvariant();
    }
}
