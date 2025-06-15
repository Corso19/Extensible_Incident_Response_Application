using System.Text.Json.Serialization;
using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Dtos
{
    public class IncidentDto
    {
        public int IncidentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime DetectedAt { get; set; }
        public string Status { get; set; }
        public IncidentType Type { get; set; }
        public int Severity { get; set; }
        public int EventId { get; set; }
        public EventDto Event { get; set; }
        public string[] Recommendations { get; set; }
    }
}