using ApiGateway.Logging.models;
using ApiGateway.RateLimiting.configPolicy.modelsDto;
using ApiGateway.RateLimiting.extractFilterValues;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApiGateway.RateLimiting.keyBuild;

public class DefaultKeyBuilder : IKeyBuilder
{
    public string BuildRedisHash(FilterDto filter)
    {
        var rawMap = ExtractFilterMap(filter);
        var sorted = rawMap
            .OrderBy(kvp => kvp.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var json = JsonSerializer.Serialize(sorted);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash);
    }

    public string BuildKey(FilterDto filter)
    {
        return "rate-limit:" + BuildRedisHash(filter);
    }

    public RuleDetails BuildRuleDetails(RateLimitRuleDto rule, FilterDto filter)
    {
        var details = new RuleDetails
        {
            Strategy = rule.StrategyName,
            Limit = rule.Limit,
            Period = TimeSpan.FromSeconds(rule.PeriodSeconds),
            Filters = ExtractFilterMap(filter)
        };

        return details;
    }

    private Dictionary<string, string> ExtractFilterMap(FilterDto filter)
    {
        var map = new Dictionary<string, string>();

        foreach (var prop in typeof(FilterDto).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (string.Equals(prop.Name, "Id", StringComparison.OrdinalIgnoreCase))
                continue;

            var value = prop.GetValue(filter);

            if (value is List<string> list && list.Count > 0)
            {
                map[prop.Name.ToLowerInvariant()] = string.Join(",", list);
            }
        }

        return map;
    }
}

