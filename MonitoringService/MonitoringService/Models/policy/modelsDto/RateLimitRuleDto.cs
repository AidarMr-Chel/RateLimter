namespace ApiGateway.RateLimiting.configPolicy.modelsDto;

public class RateLimitRuleDto
{
    public string Id { get; set; } = default!;
    public string FilterId { get; set; } = default!;
    public int Limit { get; set; }
    public int PeriodSeconds { get; set; }
    public string StrategyName { get; set; } = "FixedWindow";
    public TimeSpan Period => TimeSpan.FromSeconds(PeriodSeconds);
}
