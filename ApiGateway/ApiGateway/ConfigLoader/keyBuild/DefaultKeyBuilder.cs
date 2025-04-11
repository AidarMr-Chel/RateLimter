using ApiGateway.ConfigLoader.extractFilterValues;
using ApiGateway.ConfigLoader.models;

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

        foreach (var filter in rule.Filters)
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

        return string.Join("|", parts);
    }
}
