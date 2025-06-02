using StackExchange.Redis;
namespace ApiGateway.Redis;

public class RedisRateLimitStore : IRateLimitStore
{
    private readonly IDatabase _db;

    public RedisRateLimitStore(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }
    public async Task<int> IncrementAsync(string key)
    {
        return (int)await _db.StringIncrementAsync(key);
    }
    
    public async Task<int> GetCountAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? int.Parse(value!) : 0;
    }

    public Task SetCountAsync(string key, int count)
    {
        return _db.StringSetAsync(key, count);
    }
    
    public async Task<DateTime?> GetExpireAsync(string key)
    {
        var ttl = await _db.KeyTimeToLiveAsync(key);
        return ttl.HasValue ? DateTime.UtcNow.Add(ttl.Value) : null;
    }
    
    public Task SetExpireAsync(string key, TimeSpan ttl)
    {
        return _db.KeyExpireAsync(key, ttl);
    }

    public Task ResetAsync(string key)
    {
        return _db.KeyDeleteAsync(key);
    }
}
