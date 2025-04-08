namespace ApiGateway.ConfigLoader.models;

public class RateLimitRule
{
    public int Limit { get; set; }
    public int PeriodSeconds { get; set; }

    public TimeSpan Period => TimeSpan.FromSeconds(PeriodSeconds);
}
