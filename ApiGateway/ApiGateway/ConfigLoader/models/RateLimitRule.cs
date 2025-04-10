namespace ApiGateway.ConfigLoader.models;

public class RateLimitRule
{
    public int Limit { get; set; }
    public int PeriodSeconds { get; set; }
    public string StrategyName { get; set; } = "FixedWindow";

    public TimeSpan Period => TimeSpan.FromSeconds(PeriodSeconds);
}
