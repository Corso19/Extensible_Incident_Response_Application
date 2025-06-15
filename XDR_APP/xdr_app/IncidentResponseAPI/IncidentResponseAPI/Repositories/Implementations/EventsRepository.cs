using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IncidentResponseAPI.Repositories.Implementations
{
    public class EventsRepository : IEventsRepository
    {
        private readonly IncidentResponseContext _context;

        public EventsRepository(IncidentResponseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventsModel>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<EventsModel> GetByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task AddAsync(EventsModel eventsModel, CancellationToken cancellationToken)
        {
            await _context.Events.AddAsync(eventsModel, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(EventsModel eventsModel, CancellationToken cancellationToken)
        {
            _context.Events.Update(eventsModel);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel != null)
            {
                _context.Events.Remove(eventModel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EventsModel>> GetUnprocessedEventsAsync(CancellationToken cancellationToken)
        {
            return await _context.Events
                .Where(e => !e.isProcessed)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventsModel>> GetEventsBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            return await _context.Events
                .Where(e => e.Subject == subject)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventsModel>> GetEventsByTimestampAsync(DateTime timestamp, CancellationToken cancellationToken)
        {
            return await _context.Events
                .Where(e => e.Timestamp == timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<EventsModel>> GetEventsBySenderAsync(string sender, CancellationToken cancellationToken)
        {
            return await _context.Events
                .Where(e => e.Sender == sender)
                .ToListAsync(cancellationToken);
        }

        public async Task<EventsModel> GetByMessageIdAsync(string messageId)
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.MessageId == messageId);
        }
    }
}