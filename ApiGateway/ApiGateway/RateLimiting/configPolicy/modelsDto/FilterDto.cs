namespace ApiGateway.RateLimiting.configPolicy.modelsDto;

public class FilterDto
{
    public string Id { get; set; } = default!;
    public List<string>? Ip { get; set; }
    public List<string>? Region { get; set; }
    public List<string>? Country { get; set; }
    public List<string>? UserAgent { get; set; }
    public List<string>? HttpMethod { get; set; }
    public List<string>? Path { get; set; }
    public List<string>? ApiKey { get; set; }
    public List<string>? ClientId { get; set; }
    public List<string>? UserId { get; set; }
    public List<string>? DeviceType { get; set; }
}
