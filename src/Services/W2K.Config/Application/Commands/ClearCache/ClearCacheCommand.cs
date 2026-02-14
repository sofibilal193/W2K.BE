#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Config.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public record ClearCacheCommand(string Pattern) : IRequest;
