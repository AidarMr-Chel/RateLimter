using MonitoringService.Models;

namespace MonitoringService.Services.Absrtacts
{
    public interface ILogService
    {
        Task<LogEntry?> GetByIdAsync(string id);
        Task<List<LogEntry>> GetLatestLogsAsync(int take);
        Task<List<LogEntry>> GetFilteredLogsAsync(LogFilter filter);

    }
}