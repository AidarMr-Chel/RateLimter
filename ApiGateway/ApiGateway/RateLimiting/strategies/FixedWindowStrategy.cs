
using ApiGateway.RateLimiting.core;
using ApiGateway.RateLimiting.core.abstracts;
using ApiGateway.Redis;

namespace ApiGateway.RateLimiting.strategies;

public class FixedWindowStrategy : IRateLimitingStrategy
{
    public string Name => "FixedWindow";
    private readonly IRateLimitStore _store;

    public FixedWindowStrategy(IRateLimitStore store)
    {
        _store = store;
    }

    public async Task<bool> IsRequestAllowedAsync(RateLimitRequestContext context)
    {
        var key = context.Key;
        var count = await _store.IncrementAsync(key);

        if (count == 1)
        {
            await _store.SetExpireAsync(key, context.Period);
        }

        return count <= context.Limit;
    }

}
