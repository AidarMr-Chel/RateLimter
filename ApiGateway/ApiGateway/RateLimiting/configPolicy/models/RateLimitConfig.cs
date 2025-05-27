namespace ApiGateway.RateLimiting.configPolicy.models;

public class RateLimitConfig
{
    public List<RateLimitRule> Rules { get; set; } = new();
}
