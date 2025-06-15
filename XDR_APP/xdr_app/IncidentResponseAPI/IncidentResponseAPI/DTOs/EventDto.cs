using System.Text.Json.Serialization;

namespace IncidentResponseAPI.Dtos
{
    public class EventDto
    {
        public int EventId { get; set; }
        public int SensorId { get; set; }
        public string TypeName { get; set; } = "Email";
        public string Subject { get; set; }
        public string Sender { get; set; }
        public string Details { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Timestamp { get; set; }
        public bool isProcessed { get; set; }
        public string MessageId { get; set; }
        public ICollection<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
    }
}

