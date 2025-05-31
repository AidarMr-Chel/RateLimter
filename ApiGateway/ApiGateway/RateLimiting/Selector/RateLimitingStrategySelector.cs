using ApiGateway.RateLimiting.core.abstracts;

namespace ApiGateway.RateLimiting.Selector;

public class RateLimitingStrategySelector : IRateLimitingStrategySelector
{
    private readonly Dictionary<string, IRateLimitingStrategy> _strategies;

    public RateLimitingStrategySelector(IEnumerable<IRateLimitingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(
            s => s.Name,
            s => s,
            StringComparer.OrdinalIgnoreCase);
    }

    public IRateLimitingStrategy GetStrategy(string strategyName)
    {
        if(_strategies.TryGetValue(strategyName, out var strategy)) 
            return strategy;
        throw new InvalidOperationException($"Unknown strategy: {strategyName}");
    }
}
