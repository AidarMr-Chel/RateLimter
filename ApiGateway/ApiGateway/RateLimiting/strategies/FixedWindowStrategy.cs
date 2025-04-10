
using ApiGateway.RateLimiting.core;
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
        var expiry = await _store.GetExpireAsync(key);

        if (expiry == null || expiry < DateTime.UtcNow)
        {
            await _store.SetExpireAsync(key, context.Period);
            await _store.SetCountAsync(key, 0);
        }

        await _store.IncrementAsync(key);
        var countNow = await _store.GetCountAsync(key);
        Console.WriteLine(countNow);
        return countNow <= context.Limit;
    }
}
