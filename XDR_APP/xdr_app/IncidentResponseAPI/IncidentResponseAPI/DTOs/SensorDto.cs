namespace IncidentResponseAPI.Dtos
{
    public class SensorDto
    {
        public int SensorId { get; set; }
        public string SensorName { get; set; }
        public string Type { get; set; }
        public string Configuration { get; set; }
        public bool isEnabled { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAfter { get; set; }
        public string? LastError { get; set; }
        public int RetrievalInterval { get; set; } = 10;
        public string? LastEventMarker { get; set; }
    }
}


