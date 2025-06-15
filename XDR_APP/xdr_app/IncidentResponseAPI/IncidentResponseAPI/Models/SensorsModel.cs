using System.ComponentModel.DataAnnotations;
using IncidentResponseAPI.Helpers;

namespace IncidentResponseAPI.Models
{
    public class SensorsModel
    {
        [Key]
        public int SensorId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string SensorName { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [ValidConfiguration]
        public string Configuration { get; set; }
        public bool isEnabled { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAfter { get; set; }//for scheduling

        [StringLength(1000, ErrorMessage = "LastError cannot exceed 1000 characters.")]
        public string? LastError { get; set; } = string.Empty; //Error message from the last run, if any

        [Range(1, 1440, ErrorMessage = "Sensor number must be between 1 and 1440 minutes.")]
        public int RetrievalInterval { get; set; } = 10; //Interval in minutes for data retrieval

        [StringLength(4000, ErrorMessage = "LastEventMarker must not exceed 4000 characters.")]
        public string? LastEventMarker { get; set; } = string.Empty; //Marker for the last event processed
    }
}
