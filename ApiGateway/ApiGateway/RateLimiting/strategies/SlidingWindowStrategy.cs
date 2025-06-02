using ApiGateway.RateLimiting.core;
using ApiGateway.RateLimiting.core.abstracts;
using ApiGateway.Redis;

namespace ApiGateway.RateLimiting.strategies;

public class SlidingWindowStrategy : IRateLimitingStrategy
{
    public string Name => "SlidingWindow";

    private readonly IRateLimitStore _store;

    public SlidingWindowStrategy(IRateLimitStore store)
    {
        _store = store;
    }

    public async Task<bool> IsRequestAllowedAsync(RateLimitRequestContext context)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var period = (int)context.Period.TotalSeconds;

        var currentWindow = now / period;
        var previousWindow = currentWindow - 1;

        var currentKey = $"{context.Key}:{currentWindow}";
        var previousKey = $"{context.Key}:{previousWindow}";

        var currentCount = await _store.IncrementAsync(currentKey);

        await _store.SetExpireAsync(currentKey, context.Period * 2); 
        var previousCount = await _store.GetCountAsync(previousKey);

        var elapsed = now % period;
        var weight = (double)(period - elapsed) / period;

        var estimatedCount = previousCount * weight + currentCount;

        return estimatedCount <= context.Limit;
    }

}
