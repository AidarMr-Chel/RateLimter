using MonitoringService.Models;
using MonitoringService.Models.loging.modelsDto;
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

    public Task<List<LogEntryDto>> GetLatestLogsAsync(int take)
    {
        return _repository.GetLatestLogsAsync(take);
    }

    public Task<LogEntryDto?> GetByIdAsync(string id)
    {
        return _repository.GetByIdAsync(id)!;
    }

    public Task<List<LogEntryDto>> GetFilteredLogsAsync(LogFilterDto filter)
    {
        return _repository.GetFilteredLogsAsync(filter);
    }

}
