namespace ApiGateway.ConfigLoader.models;

public class RateLimitConfig
{
    public List<RateLimitRule> Rules { get; set; } = new();
}
