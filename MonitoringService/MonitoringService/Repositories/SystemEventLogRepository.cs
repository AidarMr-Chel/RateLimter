using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MonitoringService.Models.loging;
using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Repositories.Abstracts;

namespace MonitoringService.Repositories;

public class SystemEventLogRepository : ISystemEventLogRepository
{
    private readonly IMongoCollection<SystemEventLogEntryDto> _collection;

    public SystemEventLogRepository(IOptions<MongoSettings> options)
    {
        var setting = options.Value;
        var client = new MongoClient(setting.ConnectionString);
        var db = client.GetDatabase(setting.Database);
        _collection = db.GetCollection<SystemEventLogEntryDto>(setting.SystemEventLogEntry);
    }

    public async Task<List<SystemEventLogEntryDto>> GetEventsSinceAsync(DateTime since, int? maxCount = null)
    {
        var query = _collection.Find(e => e.Timestamp >= since);

        if (maxCount.HasValue)
            query = query.Limit(maxCount.Value);

        query = query.SortByDescending(e => e.Timestamp);

        return await query.ToListAsync();
    }
}
