using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringService.Models;
using MonitoringService.Models.loging;
using MonitoringService.Repositories.Abstracts;

namespace MonitoringService.Repositories;

public class LogRepository : ILogRepository
{
    private readonly IMongoCollection<LogEntry> _logs;

    public LogRepository(IOptions<MongoSettings> options)
    {
        var setting = options.Value;
        var client = new MongoClient(setting.ConnectionString);
        var db = client.GetDatabase(setting.Database);
        _logs = db.GetCollection<LogEntry>(setting.Collection);
    }

    public async Task<List<LogEntry>> GetLatestLogsAsync(int take)
    {
        return await _logs.Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .Limit(take)
            .ToListAsync();
    }

    public async Task<LogEntry> GetByIdAsync(string id)
    {
        return await _logs.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<LogEntry>> GetFilteredLogsAsync(LogFilter filter)
    {
        var mongoFilter = BuildFilter(filter);

        return await _logs.Find(mongoFilter)
            .SortByDescending(x => x.Timestamp)
            .Limit(filter.Take)
            .ToListAsync();
    }


    private FilterDefinition<LogEntry> BuildFilter(LogFilter filter)
    {
        var builder = Builders<LogEntry>.Filter;
        var filters = new List<FilterDefinition<LogEntry>>();

        if (!string.IsNullOrEmpty(filter.Region))
            filters.Add(builder.Eq(x => x.Region, filter.Region));
        if (!string.IsNullOrEmpty(filter.Ip))
            filters.Add(builder.Eq(x => x.Ip, filter.Ip));
        if (filter.StatusCode.HasValue)
            filters.Add(builder.Eq(x => x.StatusCode, filter.StatusCode.Value));
        if (!string.IsNullOrEmpty(filter.UserAgent))
            filters.Add(builder.Regex(x => x.UserAgent, new BsonRegularExpression(filter.UserAgent, "i")));
        if (!string.IsNullOrEmpty(filter.Reason))
            filters.Add(builder.Regex(x => x.Reason, new BsonRegularExpression(filter.Reason, "i")));
        if (filter.From.HasValue)
            filters.Add(builder.Gte(x => x.Timestamp, filter.From.Value));
        if (filter.To.HasValue)
            filters.Add(builder.Lte(x => x.Timestamp, filter.To.Value));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }

}
