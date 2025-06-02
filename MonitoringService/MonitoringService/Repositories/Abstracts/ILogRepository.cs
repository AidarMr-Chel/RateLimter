using MonitoringService.Models.loging.modelsDto;


namespace MonitoringService.Repositories.Abstracts
{
    public interface ILogRepository
    {
        Task<LogEntryDto> GetByIdAsync(string id);
        Task<List<LogEntryDto>> GetLatestLogsAsync(int take);
        Task<List<LogEntryDto>> GetFilteredLogsAsync(LogFilterDto filter);
        Task<List<LogEntryDto>> GetLogsSinceAsync(DateTime since, int? maxCount);

    }
}