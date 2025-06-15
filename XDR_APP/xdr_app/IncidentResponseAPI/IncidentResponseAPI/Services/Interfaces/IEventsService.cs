using IncidentResponseAPI.Dtos;
using Microsoft.Graph.Models;

namespace IncidentResponseAPI.Services.Interfaces
{
    public interface IEventsService
    {
        //CRUD operations
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto> GetEventByIdAsync(int id);
        Task AddEventAsync(EventDto eventDto, CancellationToken cancellationToken);
        Task UpdateEventAsync(EventDto eventDto, CancellationToken cancellationToken);
        Task DeleteEventAsync(int id);
        
        //Email/Event-related operations
        //Task SyncEventsAsync(int sensorId, CancellationToken cancellationToken);

        Task<Message> FetchMessageContentAsync(string clientSecret, string applicationId, string tenantId,
            string messageId);
        
        //Attachment-related operations
        Task<IEnumerable<AttachmentDto>> GetAttachmentsByEventIdAsync(int eventId);
        // Task AddAttachmentAsync(AttachmentDto attachmentDto);
        
    }
}




