namespace MonitoringService.Models.loging;

public class MongoSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string LogEntryCollection { get; set; } = string.Empty;
    public string SystemEventLogEntry { get; set; } = string.Empty;
}
