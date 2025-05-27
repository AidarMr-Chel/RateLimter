using ApiGateway.Logging.models;
using ApiGateway.RateLimiting.configPolicy.modelsDto;
using ApiGateway.RateLimiting.extractFilterValues;
using System.Security.Cryptography;
using System.Text;

namespace ApiGateway.RateLimiting.keyBuild;

public class DefaultKeyBuilder : IKeyBuilder
{
    private readonly IRequestFilterValueExtractor _extractor;

    public DefaultKeyBuilder(IRequestFilterValueExtractor extractor)
    {
        _extractor = extractor;
    }

    public string BuildKey(RateLimitRuleDto rule, FilterDto filter, HttpContext context)
    {
        var parts = ExtractParts(filter, context);
        if (parts.Count == 0)
        {
            var ip = _extractor.Extract("ip", context) ?? "unknown";
            parts.Add($"ip:{ip}");
        }

        return "rate-limit:" + string.Join(":", parts);
    }

    public string BuildRuleId(RateLimitRuleDto rule, FilterDto filter, HttpContext context)
    {
        var raw = BuildKey(rule, filter, context);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    public RuleDetails BuildRuleDetails(RateLimitRuleDto rule, FilterDto filter, HttpContext context)
    {
        var details = new RuleDetails
        {
            Strategy = rule.StrategyName,
            Limit = rule.Limit,
            Period = TimeSpan.FromSeconds(rule.PeriodSeconds)
        };

        foreach (var kvp in ExtractParts(filter, context))
        {
            var split = kvp.Split(':');
            if (split.Length == 2)
                details.Filters[split[0]] = split[1];
        }

        return details;
    }

    private List<string> ExtractParts(FilterDto filter, HttpContext context)
    {
        var parts = new List<string>();

        void TryAdd(string key, List<string>? expected)
        {
            if (expected is { Count: > 0 })
            {
                var val = _extractor.Extract(key, context);
                if (!string.IsNullOrWhiteSpace(val))
                    parts.Add($"{key.ToLowerInvariant()}:{val.ToLowerInvariant()}");
            }
        }

        TryAdd("ip", filter.Ip);
        TryAdd("region", filter.Region);
        TryAdd("country", filter.Country);
        TryAdd("user-agent", filter.UserAgent);
        TryAdd("httpmethod", filter.HttpMethod);
        TryAdd("path", filter.Path);
        TryAdd("apikey", filter.ApiKey);
        TryAdd("clientid", filter.ClientId);
        TryAdd("userid", filter.UserId);
        TryAdd("devicetype", filter.DeviceType);

        return parts;
    }
}

