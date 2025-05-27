using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MonitoringService.Models.loging.modelsDto;

public class LogEntryDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public DateTime Timestamp { get; set; }
    public string Ip { get; set; }
    public string Region { get; set; }
    public string Path { get; set; }
    public int StatusCode { get; set; }
    public string Reason { get; set; }
    public string UserAgent { get; set; }
}
