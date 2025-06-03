namespace MonitoringService.Models.metrics;

public class MetricDto
{
    public int TotalRequests { get; set; }
    public double Rps { get; set; }
    public int Count429 { get; set; }
    public int Count5xx { get; set; }
    public double FailureRate { get; set; }

    public double AvgLatencyMs { get; set; }

    public double RetrySuccessRate { get; set; }
    public Dictionary<string, int> RetryPerPath { get; set; } = new();
    public CircuitBreakerTriggerDto? LastCircuitBreakerTrigger { get; set; }

    public Dictionary<string, int> RequestsPerRegion { get; set; } = new();
    public Dictionary<string, int> RequestsPerInstance { get; set; } = new();
    public Dictionary<string, int> RequestsPerUserAgent { get; set; } = new();
    public Dictionary<string, int> RequestsPerPath { get; set; } = new();
}
