namespace ApiGateway.Redis;

public interface IRateLimitStore
{
    Task<int> IncrementAsync(string key);
    
    Task<int> GetCountAsync(string key);
    Task SetCountAsync(string key, int count);

    
    Task SetExpireAsync(string key, TimeSpan ttl);
    Task<DateTime?> GetExpireAsync(string key);
    
    Task ResetAsync(string key);
}


