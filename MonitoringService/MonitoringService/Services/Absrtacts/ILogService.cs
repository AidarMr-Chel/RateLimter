using MonitoringService.Models.loging.modelsDto;

namespace MonitoringService.Services.Absrtacts
{
    public interface ILogService
    {
        Task<LogEntryDto?> GetByIdAsync(string id);
        Task<List<LogEntryDto>> GetLatestLogsAsync(int take);
        Task<List<LogEntryDto>> GetFilteredLogsAsync(LogFilterDto filter);

    }
}