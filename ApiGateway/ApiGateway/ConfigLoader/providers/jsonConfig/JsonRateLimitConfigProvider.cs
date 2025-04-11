using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader.providers.jsonConfig;

public class JsonRateLimitConfigProvider : IRateLimitConfigProvider
{
    private readonly RateLimitConfigLoader _loader;

    public JsonRateLimitConfigProvider(RateLimitConfigLoader loader)
    {
        _loader = loader;
    }
    public RateLimitRule GetRule(HttpContext context)
    {
        return _loader.GetRule(context);
    }
}
