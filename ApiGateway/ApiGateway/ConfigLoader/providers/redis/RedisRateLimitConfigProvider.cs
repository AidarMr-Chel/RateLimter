using ApiGateway.ConfigLoader.extractFilterValues;
using ApiGateway.ConfigLoader.models;
using StackExchange.Redis;
using System.Text.Json;

namespace ApiGateway.ConfigLoader.providers.redis;

public class RedisRateLimitConfigProvider : IRateLimitConfigProvider
{
    private readonly IDatabase _db;
    private readonly IRequestFilterValueExtractor _extractor;

    public RedisRateLimitConfigProvider(IConnectionMultiplexer redis, 
        IRequestFilterValueExtractor extractor)
    {
        _db = redis.GetDatabase();
        _extractor = extractor;
    }

    public async Task<RateLimitRule> GetRule(HttpContext context)
    {
        var entries = await _db.HashGetAllAsync("rate-limit:rules");

        var matchingRule = entries
            .Select(entry =>
            {
                try
                {
                    return JsonSerializer.Deserialize<RateLimitRule>(entry.Value!);
                }
                catch
                {
                    return null;
                }
            })
            .Where(rule => rule != null)
            .OrderByDescending(rule => rule.Filters.Count)
            .FirstOrDefault(rule => IsMatch(rule, context));

        return matchingRule ?? throw new Exception("No matching rate limit rule found");
    }

    public bool IsMatch(RateLimitRule rule, HttpContext context)
    {
        foreach (var filter in rule.Filters)
        {
            var actual = _extractor.Extract(filter.Key, context);

            if (string.IsNullOrWhiteSpace(actual)) 
                return false;
            if (filter.Value is List<string> expected)
            {
                if (!expected.Any(val => string.Equals(actual, val, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
        }

        return true;
    }

}
