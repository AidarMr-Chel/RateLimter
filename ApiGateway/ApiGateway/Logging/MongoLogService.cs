
using MongoDB.Driver;

namespace ApiGateway.Logging;

public class MongoLogService : ILogService
{
    private readonly IMongoCollection<RateLimitLogEntry> _collection;

    public MongoLogService(IConfiguration config)
    {
        var mongoConnectionString = config.GetConnectionString("Mongo");
        var client = new MongoClient(mongoConnectionString);

        var db = client.GetDatabase("api-gateway-logs");
        _collection = db.GetCollection<RateLimitLogEntry>("rate-limit-logs");
    }


    public async Task LogAsync(RateLimitLogEntry entry)
    {
        try
        {
            await _collection.InsertOneAsync(entry);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Ошибка записи лога: {ex.Message}");
        }
    }
}
