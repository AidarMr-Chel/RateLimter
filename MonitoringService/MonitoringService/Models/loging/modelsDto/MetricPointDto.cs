namespace MonitoringService.Models.loging.modelsDto;

public class MetricPointDto
{
    public string Timestamp { get; set; } = default!;
    public double Rps { get; set; }
    public int Status429 { get; set; }
    public double FailureRate { get; set; }
    public double Latency { get; set; }
}
