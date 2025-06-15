using IncidentResponseAPI.Models;
using Microsoft.EntityFrameworkCore;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Implementations;
using Prometheus;

namespace IncidentResponseAPI.Repositories.Implementations
{
    public class IncidentsRepository : IIncidentsRepository
    {
        private readonly IncidentResponseContext _context;
        private readonly SecurityMetricsService _metricsService;

        public IncidentsRepository(IncidentResponseContext context, SecurityMetricsService metricsService)
        {
            _context = context;
            _metricsService = metricsService;
        }

        public async Task<IEnumerable<IncidentsModel>> GetAllAsync(bool includeEvent = false,
            bool includeRelations = false)
        {
            return await _context.Incidents
                .Include(i => i.Event)
                .ThenInclude(e => e.Attachments)
                .Include(i => i.Recommendations)
                .ToListAsync();
        }


        public async Task<IncidentsModel> GetByIdAsync(int id)
        {
            return await _context.Incidents
                .Include(i => i.Event)
                .ThenInclude(e => e.Attachments) // Include attachments with events
                .FirstOrDefaultAsync(i => i.IncidentId == id);
        }

        public async Task AddAsync(IncidentsModel incidentsModel, CancellationToken cancellationToken)
        {
            _context.Incidents.Add(incidentsModel);
            await _context.SaveChangesAsync(cancellationToken);
            
            _metricsService.IncidentsDetected
                .WithLabels(
                    incidentsModel.Severity.ToString(),
                    incidentsModel.Type.ToString()
                    )
                .Inc();
        }

        public async Task UpdateAsync(IncidentsModel incidentsModel)
        {
            _context.Entry(incidentsModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var incidentsModel = await _context.Incidents.FindAsync(id);
            if (incidentsModel != null)
            {
                _context.Incidents.Remove(incidentsModel);
                await _context.SaveChangesAsync();
            }
        }
    }
}