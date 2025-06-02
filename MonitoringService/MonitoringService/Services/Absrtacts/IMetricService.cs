using MonitoringService.Models.metrics;

namespace MonitoringService.Services.Absrtacts;

public interface IMetricService
{
    Task<MetricDto> GetCurrentMetricsAsync(int rangeSeconds = 60, int maxCount = 1000);

}
