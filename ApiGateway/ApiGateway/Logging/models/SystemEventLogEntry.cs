namespace ApiGateway.Logging.models;

public class SystemEventLogEntry
{
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = ""; 
    public string Message { get; set; } = "";
    public string? Path { get; set; }
    public string? InstanceId { get; set; }
}
