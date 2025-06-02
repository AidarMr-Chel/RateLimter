using ApiGateway.RateLimiting.configPolicy.modelsDto;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Helpers;
using StackExchange.Redis;
using System.Text.Json;

namespace MonitoringService.Repositories;

public class PolicyRepository : IPolicyRepository
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

    public PolicyRepository(IConnectionMultiplexer redis)
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

    public async Task<FilterDto?> GetFilterAsync(string id)
    {
        var json = await _db.StringGetAsync(FilterKeyPrefix + id);
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<FilterDto>(json!, _jsonOptions);
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

    public async Task<RateLimitRuleDto?> GetRuleAsync(string id)
    {
        var json = await _db.StringGetAsync(RuleKeyPrefix + id);
        return json.IsNullOrEmpty ? null : JsonSerializer.Deserialize<RateLimitRuleDto>(json!, _jsonOptions);
    }

    public async Task SaveRuleAsync(RateLimitRuleDto rule, FilterDto filter)
    {
        if (string.IsNullOrWhiteSpace(filter.Id))
            filter.Id = "auto-" + FilterIdGenerator.ComputeHash(filter);

        var jsonFilter = JsonSerializer.Serialize(filter, _jsonOptions);
        await _db.SetAddAsync(FiltersSetKey, filter.Id);
        await _db.StringSetAsync(FilterKeyPrefix + filter.Id, jsonFilter);

        rule.FilterId = filter.Id;
        var jsonRule = JsonSerializer.Serialize(rule, _jsonOptions);
        await _db.SetAddAsync(RulesSetKey, rule.Id);
        await _db.StringSetAsync(RuleKeyPrefix + rule.Id, jsonRule);
    }

    public async Task DeleteRuleAsync(string id)
    {
        var rule = await GetRuleAsync(id);
        if (rule == null)
            return;

        await _db.SetRemoveAsync(RulesSetKey, id);
        await _db.KeyDeleteAsync(RuleKeyPrefix + id);

        await _db.SetRemoveAsync(FiltersSetKey, rule.FilterId);
        await _db.KeyDeleteAsync(FilterKeyPrefix + rule.FilterId);
    }

}