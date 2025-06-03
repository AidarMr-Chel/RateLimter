namespace MonitoringService.Repositories.Abstracts;

public interface IRpsMetricsRepository
{
    Task<int> GetAndResetRpsAsync(string key = "rps:total");
}
