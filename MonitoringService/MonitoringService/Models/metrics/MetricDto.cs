namespace MonitoringService.Models.metrics;

public class MetricDto
{
    public int TotalRequests { get; set; }
    public double Rps { get; set; }
    public int Count429 { get; set; }
    public int Count5xx { get; set; }
    public double FailureRate => TotalRequests == 0 ? 0 : Math.Round((Count429 + Count5xx) * 100.0 / TotalRequests, 2);
    public double AvgLatencyMs { get; set; }

    public Dictionary<string, int> RequestsPerRegion { get; set; } = new();
    public Dictionary<string, int> RequestsPerInstance { get; set; } = new();
    public Dictionary<string, int> RequestsPerUserAgent { get; set; } = new();
    public Dictionary<string, int> RequestsPerPath { get; set; } = new();
}
