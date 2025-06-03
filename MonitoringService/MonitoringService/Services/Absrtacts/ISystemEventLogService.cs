using MonitoringService.Models.loging.modelsDto;

namespace MonitoringService.Services.Absrtacts;

public interface ISystemEventLogService
{
    Task<List<SystemEventLogEntryDto>> GetRecentEventsAsync(int minutes);
    Task<List<SystemEventLogEntryDto>> FilterEventsAsync(string? type, string? path, int minutes);
}
