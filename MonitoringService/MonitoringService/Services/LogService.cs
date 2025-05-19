using MonitoringService.Models;
using MonitoringService.Repositories;
using MonitoringService.Repositories.Abstracts;
using MonitoringService.Services.Absrtacts;

namespace MonitoringService.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _repository;

    public LogService(ILogRepository repository)
    {
        _repository = repository;
    }

    public Task<List<LogEntry>> GetLatestLogsAsync(int take)
    {
        return _repository.GetLatestLogsAsync(take);
    }

    public Task<LogEntry?> GetByIdAsync(string id)
    {
        return _repository.GetByIdAsync(id)!;
    }

    public Task<List<LogEntry>> GetFilteredLogsAsync(LogFilter filter)
    {
        return _repository.GetFilteredLogsAsync(filter);
    }

}
