namespace ApiGateway.RateLimiting.core;

public interface IRateLimitingStrategySelector
{
    IRateLimitingStrategy GetStrategy(string strategyName);
}
