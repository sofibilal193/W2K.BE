using System.Collections.ObjectModel;

namespace DFI.Common.Application.Settings;

public readonly record struct ApiVersionOptions(Collection<ApiVersion> Versions, ApiVersion DefaultVersion);

public readonly record struct ApiVersion(int MajorVersion = 1, int MinorVersion = 0);
