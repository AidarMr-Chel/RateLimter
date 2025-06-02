namespace MonitoringService.Models.policy.modelsDto;

public class ValidationResultDto
{
    public bool IsSuccess { get; set; }
    public List<string> Errors { get; set; } = new();
}

