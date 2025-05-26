using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ApiGateway.Logging.models;

public class LogEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Ip { get; set; }
    public string Region { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }

    public int StatusCode { get; set; }
    public string Reason { get; set; }

    public string? RuleId { get; set; }
    public RuleDetails? Rule { get; set; }

    public int? DurationMs { get; set; }

    public Dictionary<string, string>? Headers { get; set; }
}


