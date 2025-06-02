using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MonitoringService.Models.loging;
using MonitoringService.Models.loging.modelsDto;
using MonitoringService.Repositories.Abstracts;

namespace MonitoringService.Repositories;

public class LogRepository : ILogRepository
{
    private readonly IMongoCollection<LogEntryDto> _logs;

    public LogRepository(IOptions<MongoSettings> options)
    {
        var setting = options.Value;
        var client = new MongoClient(setting.ConnectionString);
        var db = client.GetDatabase(setting.Database);
        _logs = db.GetCollection<LogEntryDto>(setting.Collection);
    }

    public async Task<List<LogEntryDto>> GetLatestLogsAsync(int take)
    {
        return await _logs.Find(_ => true)
            .SortByDescending(x => x.Timestamp)
            .Limit(take)
            .ToListAsync();
    }

    public async Task<LogEntryDto> GetByIdAsync(string id)
    {
        return await _logs.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<LogEntryDto>> GetFilteredLogsAsync(LogFilterDto filter)
    {
        var mongoFilter = BuildFilter(filter);

        return await _logs.Find(mongoFilter)
        .SortByDescending(x => x.Timestamp)
        .Skip(filter.Skip)
        .Limit(filter.Take)
        .ToListAsync();
    }

    public async Task<List<LogEntryDto>> GetLogsSinceAsync(DateTime since, int? maxCount = null)
    {
        var query = _logs.Find(x => x.Timestamp >= since);

        if (maxCount.HasValue)
            query = query.Limit(maxCount.Value);

        query = query.SortByDescending(x => x.Timestamp);

        return await query.ToListAsync();
    }

    private FilterDefinition<LogEntryDto> BuildFilter(LogFilterDto filter)
    {
        var builder = Builders<LogEntryDto>.Filter;
        var filters = new List<FilterDefinition<LogEntryDto>>();

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

        if (!string.IsNullOrEmpty(filter.InstanceId))
            filters.Add(builder.Eq(x => x.InstanceId, filter.InstanceId));

        if (!string.IsNullOrEmpty(filter.Path))
            filters.Add(builder.Regex(x => x.Path, new BsonRegularExpression(filter.Path, "i")));

        if (!string.IsNullOrEmpty(filter.Method))
            filters.Add(builder.Eq(x => x.Method, filter.Method));

        return filters.Count > 0 ? builder.And(filters) : builder.Empty;
    }


}
