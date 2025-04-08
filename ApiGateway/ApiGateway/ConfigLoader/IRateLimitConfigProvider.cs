using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader;

public interface IRateLimitConfigProvider
{
    RateLimitRule GetRule(string? region = null);
}
