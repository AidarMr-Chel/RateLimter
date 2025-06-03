using MonitoringService.Models.loging.modelsDto;

namespace MonitoringService.Repositories.Abstracts;

public interface ISystemEventLogRepository
{
    Task<List<SystemEventLogEntryDto>> GetEventsSinceAsync(DateTime since, int? maxCount = null);
}
