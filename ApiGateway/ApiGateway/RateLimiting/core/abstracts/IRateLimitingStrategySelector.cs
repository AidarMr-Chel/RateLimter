namespace ApiGateway.RateLimiting.core.abstracts;

public interface IRateLimitingStrategySelector
{
    IRateLimitingStrategy GetStrategy(string strategyName);
}
