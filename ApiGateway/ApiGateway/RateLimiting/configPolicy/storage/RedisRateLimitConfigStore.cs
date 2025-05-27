
using ApiGateway.RateLimiting.configPolicy.abstracts;
using ApiGateway.RateLimiting.configPolicy.modelsDto;
using StackExchange.Redis;
using System.Text.Json;

namespace ApiGateway.RateLimiting.configPolicy.storage;

public class RedisRateLimitConfigStore : IRateLimitConfigStore
{
    private readonly IDatabase _db;

    private const string FiltersSetKey = "ratelimit:filters";
    private const string RulesSetKey = "ratelimit:rules";
    private const string FilterKeyPrefix = "ratelimit:filter:";
    private const string RuleKeyPrefix = "ratelimit:rule:";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public RedisRateLimitConfigStore(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<IEnumerable<FilterDto>> GetAllFiltersAsync()
    {
        var ids = await _db.SetMembersAsync(FiltersSetKey);
        var result = new List<FilterDto>();

        foreach (var id in ids.Select(i => (string)i))
        {
            var json = await _db.StringGetAsync(FilterKeyPrefix + id);
            if (!json.IsNullOrEmpty)
            {
                var filter = JsonSerializer.Deserialize<FilterDto>(json!, _jsonOptions);
                if (filter != null)
                    result.Add(filter);
            }
        }

        return result;
    }

    public async Task<IEnumerable<RateLimitRuleDto>> GetAllRulesAsync()
    {
        var ids = await _db.SetMembersAsync(RulesSetKey);
        var result = new List<RateLimitRuleDto>();

        foreach (var id in ids.Select(i => (string)i))
        {
            var json = await _db.StringGetAsync(RuleKeyPrefix + id);
            if (!json.IsNullOrEmpty)
            {
                var rule = JsonSerializer.Deserialize<RateLimitRuleDto>(json!, _jsonOptions);
                if (rule != null)
                    result.Add(rule);
            }
        }

        return result;
    }

}