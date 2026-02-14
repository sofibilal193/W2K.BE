using W2K.Common.Application.Cacheing;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace W2K.Config.Application;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public class ClearCacheCommandHandler(
    ICache cache,
    ILogger<ClearCacheCommandHandler> logger) : IRequestHandler<ClearCacheCommand>
{
    private readonly ICache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<ClearCacheCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(ClearCacheCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clearing all cache entries for {Pattern}.", request.Pattern);

        await _cache.RemoveAllAsync("*", request.Pattern, cancellationToken);

        _logger.LogInformation("Successfully cleared all cache entries for pattern '{Pattern}'", request.Pattern);
    }
}
