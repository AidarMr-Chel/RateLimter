using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Models.metrics;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Services;

public class MetricsService : IMetricService
{
    private readonly ILogRepository _repository;

    public MetricsService(ILogRepository logRepo)
    {
        _repository = logRepo;
    }

    public async Task<MetricDto> GetCurrentMetricsAsync(int rangeSeconds = 60, int maxCount = 1000)
    {
        var since = DateTime.UtcNow.AddSeconds(-rangeSeconds);
        var logs = await _repository.GetLogsSinceAsync(since, maxCount); 

        var dto = new MetricDto
        {
            TotalRequests = logs.Count,
            Count429 = logs.Count(l => l.StatusCode == 429),
            Count5xx = logs.Count(l => l.StatusCode >= 500),
            AvgLatencyMs = logs.Where(l => l.DurationMs.HasValue).DefaultIfEmpty()
                               .Average(l => l?.DurationMs ?? 0)
        };

        dto.Rps = rangeSeconds > 0 ? Math.Round(logs.Count / (double)rangeSeconds, 2) : 0;

        dto.RequestsPerRegion = logs
            .Where(l => !string.IsNullOrWhiteSpace(l.Region))
            .GroupBy(l => l.Region)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.RequestsPerInstance = logs
            .Where(l => !string.IsNullOrWhiteSpace(l.InstanceId))
            .GroupBy(l => l.InstanceId!)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.RequestsPerUserAgent = logs
            .GroupBy(l => l.UserAgent)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());

        dto.RequestsPerPath = logs
            .Where(l => !string.IsNullOrWhiteSpace(l.Path))
            .GroupBy(l => l.Path)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());

        return dto;
    }

    public async Task<List<MetricPointDto>> GetMetricSeriesAsync(int rangeSeconds = 60, int maxCount = 1000)
    {
        var since = DateTime.UtcNow.AddSeconds(-rangeSeconds);
        var logs = await _repository.GetLogsSinceAsync(since, maxCount);

        var points = Enumerable
            .Range(0, rangeSeconds)
            .Select(offset =>
            {
                var from = since.AddSeconds(offset);
                var to = from.AddSeconds(1);

                var logsInSecond = logs
                    .Where(l => l.Timestamp >= from && l.Timestamp < to)
                    .ToList();

                var count = logsInSecond.Count;
                var count429 = logsInSecond.Count(l => l.StatusCode == 429);
                var count5xx = logsInSecond.Count(l => l.StatusCode >= 500);
                var latency = logsInSecond.Where(l => l.DurationMs.HasValue).DefaultIfEmpty()
                                          .Average(l => l?.DurationMs ?? 0);

                return new MetricPointDto
                {
                    Timestamp = from.ToLocalTime().ToString("HH:mm:ss"),
                    Rps = count,
                    Status429 = count429,
                    FailureRate = count == 0 ? 0 : Math.Round((double)count5xx / count * 100, 2),
                    Latency = Math.Round(latency, 2)
                };
            })
            .ToList();

        return points;
    }


}
