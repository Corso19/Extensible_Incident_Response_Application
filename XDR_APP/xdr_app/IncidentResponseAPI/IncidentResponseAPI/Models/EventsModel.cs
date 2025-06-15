using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace IncidentResponseAPI.Models
{
    public class EventsModel
    {
        [Key]
        public int EventId { get; set; }
        public int SensorId { get; set; }
        public SensorsModel Sensor { get; set; }
        public string TypeName { get; set; } = "Email";
        public string Subject { get; set; }
        public string Sender { get; set; }
        public string Details { get; set; } //Would be better to be called message
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool isProcessed { get; set; } = false;
        public string MessageId { get; set; }   //Microsoft Graph MessageId
        public ICollection<AttachmentModel> Attachments { get; set; } = new List<AttachmentModel>();
        //Navigation property for related Incidents
        public ICollection<IncidentsModel> Incidents { get; set; } = new List<IncidentsModel>();
    }
}
