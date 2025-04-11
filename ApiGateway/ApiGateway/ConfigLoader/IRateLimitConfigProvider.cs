using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader;

public interface IRateLimitConfigProvider
{
    RateLimitRule GetRule(HttpContext context);
}
