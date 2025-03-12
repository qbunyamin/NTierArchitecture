using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace NTierArchitecture.Business.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>,ICacheable
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType();
        _logger.LogInformation($"denemeeeee {requestName}");
       
        TResponse response = await next();
        if (_cache.TryGetValue(request.CacheKey, out response))
        {
            _logger.LogInformation($" response kısmı  {requestName}");
            return response;
        }

        _logger.LogInformation($"{requestName}");
        response = await next();
        _cache.Set(request.CacheKey, response);

        return response;
    }
}
