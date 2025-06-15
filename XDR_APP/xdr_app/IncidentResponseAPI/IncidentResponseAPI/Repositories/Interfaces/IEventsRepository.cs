using IncidentResponseAPI.Models;
using Microsoft.Build.Experimental.ProjectCache;

namespace IncidentResponseAPI.Repositories.Interfaces
{
    public interface IEventsRepository
    {
        Task<IEnumerable<EventsModel>> GetAllAsync();
        Task<EventsModel> GetByIdAsync(int id);
        Task AddAsync(EventsModel eventsModel, CancellationToken cancellationToken);
        Task UpdateAsync(EventsModel eventsModel, CancellationToken cancellationToken);
        Task DeleteAsync(int id);
        Task<IEnumerable<EventsModel>> GetUnprocessedEventsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<EventsModel>> GetEventsBySubjectAsync(string subject, CancellationToken cancellationToken);
        Task<IEnumerable<EventsModel>> GetEventsByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken);
        Task<IEnumerable<EventsModel>> GetEventsBySenderAsync(string sender, CancellationToken cancellationToken);
        
        // Task<IEnumerable<AttachmentModel>> GetAttachmentsByEventIdAsync(int eventId);
        Task<EventsModel> GetByMessageIdAsync(string messageId);
        
    }
}




