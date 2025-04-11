using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader.keyBuild;

public interface IKeyBuilder
{
    string BuildKey(RateLimitRule rule, HttpContext context);
}
