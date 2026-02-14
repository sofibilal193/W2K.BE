using System.Collections.ObjectModel;

namespace W2K.Common.Application.Settings;

public readonly record struct ApiVersionOptions(Collection<ApiVersion> Versions, ApiVersion DefaultVersion);

public readonly record struct ApiVersion(int MajorVersion = 1, int MinorVersion = 0);
