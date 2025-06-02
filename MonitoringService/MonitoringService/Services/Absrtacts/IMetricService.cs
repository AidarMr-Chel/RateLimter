using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Models.metrics;

namespace MonitoringService.Services.Absrtacts;

public interface IMetricService
{
    Task<MetricDto> GetCurrentMetricsAsync(int rangeSeconds = 60, int maxCount = 1000);
    Task<List<MetricPointDto>> GetMetricSeriesAsync(int rangeSeconds = 60, int maxCount = 1000);


}
