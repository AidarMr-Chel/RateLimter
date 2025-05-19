namespace MonitoringService.Models.loging
{
    public class LogFilter
    {
        public string? Region { get; set; }
        public string? Ip { get; set; }
        public int? StatusCode { get; set; }
        public string? UserAgent { get; set; }
        public string? Reason { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Take { get; set; } = 100;
    }
}
