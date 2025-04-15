using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader;

public interface IRateLimitConfigProvider
{
    Task<RateLimitRule> GetRule(HttpContext context);
}
