
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonitoringService.Models.loging.modelsDto;
public class SystemEventLogEntryDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Path { get; set; }
    public string? InstanceId { get; set; }
}
