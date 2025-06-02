namespace MonitoringService.Models.loging.modelsDto
{
    public class LogFilterDto
    {
        public string? Region { get; set; }
        public string? Ip { get; set; }
        public int? StatusCode { get; set; }
        public string? UserAgent { get; set; }
        public string? Reason { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public string? InstanceId { get; set; }
        public string? Path { get; set; }
        public string? Method { get; set; }

        public int Skip { get; set; } = 0;

        private int _take = 100;
        public int Take
        {
            get => Math.Clamp(_take, 1, 1000);
            set => _take = value;
        }
    }

}
