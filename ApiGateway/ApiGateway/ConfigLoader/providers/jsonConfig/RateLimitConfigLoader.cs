using ApiGateway.ConfigLoader.extractFilterValues;
using ApiGateway.ConfigLoader.models;
using System.Text.Json;

namespace ApiGateway.ConfigLoader.providers.jsonConfig;

public class RateLimitConfigLoader
{
    private readonly RateLimitConfig _config;
    private readonly IRequestFilterValueExtractor _extractor;

    public RateLimitConfigLoader(string filePath, IRequestFilterValueExtractor extractor)
    {
        var json = File.ReadAllText(filePath);
        _config = JsonSerializer.Deserialize<RateLimitConfig>(json)
             ?? throw new Exception("Invalid rate-limit-config.json");
        _extractor = extractor;
    }

    public RateLimitRule GetRule(HttpContext context)
    {
        var matchingRule = _config.Rules
            .OrderByDescending(rule => rule.Filters.Count())
            .FirstOrDefault(rule => IsMatch(rule, context, _extractor));
            
        return matchingRule ?? throw new Exception("No rate limit rules found");
    }

    public bool IsMatch(
        RateLimitRule rule, 
        HttpContext context, 
        IRequestFilterValueExtractor extractor)
    {
        foreach (var filter in rule.Filters)
        {
            var actual = extractor.Extract(filter.Key, context);

            if (string.IsNullOrWhiteSpace(actual)) 
                return false;

            if (filter.Value is List<string> expectedList)
            {
                if (!expectedList.Any(val => string.Equals(val, actual, StringComparison.OrdinalIgnoreCase))) 
                    return false;
            }
        }

        return true;
    }
}
