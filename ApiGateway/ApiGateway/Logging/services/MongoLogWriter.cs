using ApiGateway.Logging.abstracts;
using ApiGateway.Logging.models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApiGateway.Logging.services;

public class MongoLogWriter : ILogWriter
{
    private readonly IMongoDatabase _db;

    public MongoLogWriter(IOptions<MongoSettings> options)
    {
        var setting = options.Value;
        var client = new MongoClient(setting.ConnectionString);
        _db = client.GetDatabase(setting.Database);
    }

    public async Task WriteAsync<T>(T entry)
    {
        try
        {
            var collectionName = typeof(T).Name;
            var collection = _db.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(entry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing log: {ex.Message}");
        }
    }
}

