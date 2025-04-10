namespace ApiGateway.RateLimiting.core;

public class RateLimitRequestContext
{
    public string Key { get; set; } = default!;
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }


    public string? Region { get; set; }
    public string? UserAgent { get; set; }
    public string? Path { get; set; }
    public int? BurstSize { get; set; }
    public double? RefillRatePerSec { get; set; }
}
