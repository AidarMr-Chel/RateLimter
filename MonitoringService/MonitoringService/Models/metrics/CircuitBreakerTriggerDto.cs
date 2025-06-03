namespace MonitoringService.Models.metrics;

public class CircuitBreakerTriggerDto
{
    public string Path { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
