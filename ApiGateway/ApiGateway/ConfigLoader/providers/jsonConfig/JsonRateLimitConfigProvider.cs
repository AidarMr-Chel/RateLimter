using ApiGateway.ConfigLoader.models;

namespace ApiGateway.ConfigLoader.providers.jsonConfig;

public class JsonRateLimitConfigProvider : IRateLimitConfigProvider
{
    private readonly RateLimitConfigLoader _loader;

    public JsonRateLimitConfigProvider(RateLimitConfigLoader loader)
    {
        _loader = loader;
    }
    public async Task<RateLimitRule> GetRule(HttpContext context)
    {
        return _loader.GetRule(context);
    }
}
