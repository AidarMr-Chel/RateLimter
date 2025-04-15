namespace ApiGateway.Logging;

public class RateLimitLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Ip { get; set; } = default!;
    public string? Region { get; set; }
    public string Path { get; set; } = default!;
    public string? UserAgent { get; set; }
    public int StatusCode { get; set; }
    public string Reason { get; set; } = default!;
}

