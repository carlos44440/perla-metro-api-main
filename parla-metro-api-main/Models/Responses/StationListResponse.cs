namespace parla_metro_api_main.Models.Responses
{
    public class StationListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<object>? Data { get; set; }
        public int Count { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}