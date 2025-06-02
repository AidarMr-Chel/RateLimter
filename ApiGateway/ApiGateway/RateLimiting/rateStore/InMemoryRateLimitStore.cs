using System.Collections.Concurrent;

namespace ApiGateway.RateLimiting.rateStore;

public class InMemoryRateLimitStore : IRateLimitStore
{
    private class StoreItem
    {
        public int Count { get; set; }
        public DateTime? Expiry { get; set; }
    }

    private readonly ConcurrentDictionary<string, StoreItem> _store = new();

    public Task<int> IncrementAsync(string key)
    {
        var item = _store.GetOrAdd(key, _ => new StoreItem());
        item.Count++;
        return Task.FromResult(item.Count);
    }

    public Task<int> GetCountAsync(string key)
    {
        if (_store.TryGetValue(key, out var item))
            return Task.FromResult(item.Count);
        return Task.FromResult(0);
    }

    public Task SetCountAsync(string key, int count)
    {
        var item = _store.GetOrAdd(key, _ => new StoreItem());
        item.Count = count;
        return Task.CompletedTask;
    }

    public Task<DateTime?> GetExpireAsync(string key)
    {
        if (_store.TryGetValue(key, out var item))
            return Task.FromResult(item.Expiry);
        return Task.FromResult<DateTime?>(null);
    }

    public Task SetExpireAsync(string key, TimeSpan ttl)
    {
        var item = _store.GetOrAdd(key, _ => new StoreItem());
        item.Expiry = DateTime.UtcNow.Add(ttl);
        return Task.CompletedTask;
    }

    public Task ResetAsync(string key)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
