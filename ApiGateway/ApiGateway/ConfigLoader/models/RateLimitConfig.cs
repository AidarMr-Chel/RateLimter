namespace ApiGateway.ConfigLoader.models;

public class RateLimitConfig
{
    public RateLimitRule Global { get; set; } = new();
    public Dictionary<string, RateLimitRule> Region { get; set; } = new();
}
