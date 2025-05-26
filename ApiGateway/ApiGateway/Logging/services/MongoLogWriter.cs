using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiGateway.Logging.services;

public class MongoLogWriter : ILogWriter
{
    private readonly IMongoCollection<LogEntry> _logs;

    public MongoLogWriter(IOptions<MongoSettings> options)
    {
        var setting = options.Value;
        var client = new MongoClient(setting.ConnectionString);
        var db = client.GetDatabase(setting.Database);
        _logs = db.GetCollection<LogEntry>(setting.Collection);
    }

    public async Task WriteAsync(LogEntry entry)
    {
        try
        {
            await _logs.InsertOneAsync(entry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error write log: {ex.Message}");
        }
    }
}
