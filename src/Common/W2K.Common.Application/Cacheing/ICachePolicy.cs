using MediatR;

namespace W2K.Common.Application.Cacheing;

// https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
public interface ICachePolicy<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    // the name of the application or service using this cache policy
    string AppName { get; }

    DateTimeOffset? AbsoluteExpiration(TRequest request, TResponse response)
    {
        return null;
    }

    TimeSpan? AbsoluteExpirationRelativeToNow(TRequest request, TResponse response)
    {
        return null;
    }

    TimeSpan? SlidingExpiration(TRequest request, TResponse response)
    {
        return TimeSpan.FromMinutes(AppConstants.DefaultSlidingCachePolicyMinutes);
    }

    string GetCacheKey(TRequest request)
    {
        var r = new { request };
        var props = r.request.GetType().GetProperties().Select(x => $"{x.Name}:{x.GetValue(r.request, null)}");
        return $"{typeof(TRequest).FullName}{{{String.Join(",", props)}}}";
    }
}
