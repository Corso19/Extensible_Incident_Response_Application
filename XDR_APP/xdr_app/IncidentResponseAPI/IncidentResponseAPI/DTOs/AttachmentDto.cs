namespace IncidentResponseAPI.Dtos;

public class AttachmentDto
{
    public int AttachmentId { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public byte[] Content { get; set; }
    public int EventId { get; set; }
}