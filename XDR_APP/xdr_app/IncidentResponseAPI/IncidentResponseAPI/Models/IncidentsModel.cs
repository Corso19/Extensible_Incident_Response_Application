using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentResponseAPI.Models
{
    public enum IncidentType
    {
        UnusualEmailVolume,
        SuspiciousAttachment,
        ExternalSender,
        RepeatedEventPattern
    }
    
    public class IncidentsModel
    {

        [Key]
        public int IncidentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DetectedAt { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public IncidentType Type { get; set; }
        public int Severity { get; set; }
        
        //Foreign Key for Event
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public EventsModel Event { get; set; }
        public virtual ICollection<RecommendationsModel> Recommendations { get; set; }
    }
}
