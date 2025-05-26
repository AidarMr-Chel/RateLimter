using ApiGateway.ConfigLoader.extractFilterValues;
using ApiGateway.ConfigLoader.models;
using ApiGateway.Logging.models;
using System.Security.Cryptography;
using System.Text;

namespace ApiGateway.ConfigLoader.keyBuild;

public class DefaultKeyBuilder : IKeyBuilder
{
    private readonly IRequestFilterValueExtractor _extractor;

    public DefaultKeyBuilder(IRequestFilterValueExtractor extractor)
    {
        _extractor = extractor;
    }

    public string BuildKey(RateLimitRule rule, HttpContext context)
    {
        var parts = new List<string>();

        foreach (var filter in rule.Filters.OrderBy(kvp => kvp.Key))
        {
            var value = _extractor.Extract(filter.Key, context);
            if (!string.IsNullOrWhiteSpace(value))
            {
                parts.Add($"{filter.Key.ToLowerInvariant()}:{value.ToLowerInvariant()}");
            }
        }

        if (parts.Count == 0)
        {
            var ip = _extractor.Extract("Ip", context) ?? "unknown";
            parts.Add($"ip:{ip}");
        }

        return "rate-limit:" + string.Join(":", parts);
    }

    public string BuildRuleId(RateLimitRule rule, HttpContext context)
    {
        var raw = BuildKey(rule, context);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    public RuleDetails BuildRuleDetails(RateLimitRule rule, HttpContext context)
    {
        var details = new RuleDetails
        {
            Strategy = rule.StrategyName,
            Limit = rule.Limit,
            Period = rule.Period
        };

        foreach (var kvp in rule.Filters)
        {
            var val = _extractor.Extract(kvp.Key, context);
            if (!string.IsNullOrWhiteSpace(val))
                details.Filters[kvp.Key] = val;
        }

        return details;
    }
}
