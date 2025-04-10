using ApiGateway.ConfigLoader.models;
using System.Text.Json;

namespace ApiGateway.ConfigLoader.providers.jsonConfig;

public class RateLimitConfigLoader
{
    private readonly RateLimitConfig _config;

    public RateLimitConfigLoader(string filePath)
    {
        var json = File.ReadAllText(filePath);
        _config = JsonSerializer.Deserialize<RateLimitConfig>(json)
             ?? throw new Exception("Invalid rate-limit-config.json");
    }

    public RateLimitRule GetRule(string? region = null)
    {
        if (region != null && _config.Region.TryGetValue(region, out var rule))
        {
            return rule;
        }
        return _config.Global;
    }
}
