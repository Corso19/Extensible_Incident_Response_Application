using System.ComponentModel.DataAnnotations;

namespace IncidentResponseAPI.Models
{
    public class RecommendationsModel
    {
        [Key]
        public int RecommendationId { get; set; }
        public int IncidentId { get; set; }
        public IncidentsModel Incident { get; set; }
        public string Description { get; set; }
        public bool isCompleted { get; set; } = false;
    }
}
