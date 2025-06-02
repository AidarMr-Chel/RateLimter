namespace ApiGateway.Logging.models;

public class RuleDetails
{
    public string? RuleId { get; set; }
    public string Strategy { get; set; } = null!;
    public int Limit { get; set; }
    public TimeSpan Period { get; set; }
    public Dictionary<string, string> Filters { get; set; } = new();
}
