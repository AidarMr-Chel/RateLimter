using MonitoringService.Models;
using MonitoringService.Models.loging;


namespace MonitoringService.Repositories.Abstracts
{
    public interface ILogRepository
    {
        Task<LogEntry> GetByIdAsync(string id);
        Task<List<LogEntry>> GetLatestLogsAsync(int take);
        Task<List<LogEntry>> GetFilteredLogsAsync(LogFilter filter);

    }
}