namespace ApiGateway.RateLimiting.configPolicy.models;

public class RateLimitRule
{
    public Dictionary<string, List<string>> Filters { get; set; } = new();
    public int Limit { get; set; }
    public int PeriodSeconds { get; set; }
    public string StrategyName { get; set; } = "FixedWindow";

    public TimeSpan Period => TimeSpan.FromSeconds(PeriodSeconds);
}
