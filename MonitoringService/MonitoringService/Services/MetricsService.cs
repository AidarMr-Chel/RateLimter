using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Models.metrics;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Services;

public class MetricsService : IMetricService
{
    private readonly ILogRepository _repository;
    private readonly IRpsMetricsRepository _rpsRepo;
    private readonly ISystemEventLogRepository _eventRepo;

    public MetricsService(
        ILogRepository logRepo,
        IRpsMetricsRepository rpsRepo,
        ISystemEventLogRepository eventRepo)
    {
        _repository = logRepo;
        _rpsRepo = rpsRepo;
        _eventRepo = eventRepo;
    }

    public async Task<MetricDto> GetCurrentMetricsAsync(int rangeSeconds = 60, int maxCount = 1000)
    {
        var since = DateTime.UtcNow.AddSeconds(-rangeSeconds);
        var logs = await _repository.GetLogsSinceAsync(since, maxCount);
        var events = await _eventRepo.GetEventsSinceAsync(since, maxCount);

        var dto = new MetricDto
        {
            Rps = await _rpsRepo.GetAndResetRpsAsync()
        };

        CalculateCoreMetrics(logs, dto);
        CalculateGroupings(logs, dto);
        CalculateRetryMetrics(events, dto);
        CalculateCircuitBreakerMetrics(events, dto);

        return dto;
    }

    private void CalculateCoreMetrics(List<LogEntryDto> logs, MetricDto dto)
    {
        dto.TotalRequests = logs.Count;
        dto.Count429 = logs.Count(l => l.StatusCode == 429);
        dto.Count5xx = logs.Count(l => l.StatusCode >= 500);

        dto.AvgLatencyMs = logs
            .Where(l => l.DurationMs.HasValue)
            .DefaultIfEmpty()
            .Average(l => l?.DurationMs ?? 0);

        dto.FailureRate = dto.TotalRequests == 0
            ? 0
            : Math.Round((double)dto.Count5xx / dto.TotalRequests * 100, 2);
    }

    private void CalculateGroupings(List<LogEntryDto> logs, MetricDto dto)
    {
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
    }

    private void CalculateRetryMetrics(List<SystemEventLogEntryDto> events, MetricDto dto)
    {
        var retryEvents = events
            .Where(e => e.EventType == "RetryAttempt")
            .ToList();

        dto.RetrySuccessRate = retryEvents.Count == 0
            ? 0
            : 100;

        dto.RetryPerPath = retryEvents
            .Where(e => !string.IsNullOrWhiteSpace(e.Path))
            .GroupBy(e => e.Path!)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private void CalculateCircuitBreakerMetrics(List<SystemEventLogEntryDto> events, MetricDto dto)
    {
        var cbEvent = events
            .Where(e => e.EventType == "CircuitBreakerOpen")
            .OrderByDescending(e => e.Timestamp)
            .FirstOrDefault();

        if (cbEvent is not null)
        {
            dto.LastCircuitBreakerTrigger = new CircuitBreakerTriggerDto
            {
                Path = cbEvent.Path ?? "unknown",
                Instance = cbEvent.InstanceId ?? "unknown",
                Timestamp = cbEvent.Timestamp
            };
        }
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
