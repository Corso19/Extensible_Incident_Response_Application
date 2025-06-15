using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IncidentResponseAPI.Models;

public class AttachmentModel
{
    [Key]
    public int AttachmentId { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public byte[] Content { get; set; }
    [Required]
    public int EventId { get; set; }
    [ForeignKey(nameof(EventId))]
    public EventsModel Event { get; set; } //Navigation property for the Event
    
}