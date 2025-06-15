using IncidentResponseAPI.Models;
using Microsoft.EntityFrameworkCore;
using IncidentResponseAPI.Repositories.Interfaces;

namespace IncidentResponseAPI.Repositories.Implementations
{
    public class SensorsRepository : ISensorsRepository
    {
        private readonly IncidentResponseContext _context;

        public SensorsRepository(IncidentResponseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SensorsModel>> GetAllAsync()
        {
            return await _context.Sensors.ToListAsync();
        }

        public async Task<SensorsModel> GetByIdAsync(int id)
        {
            return await _context.Sensors.FindAsync(id);
        }

        public async Task AddAsync(SensorsModel sensorsModel)
        {
            _context.Sensors.Add(sensorsModel);
            await _context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<SensorsModel>> GetAllEnabledAsync()
        {
            return await _context.Sensors
                .Where(s => s.isEnabled)
                .ToListAsync();
        }

        public async Task UpdateAsync(SensorsModel sensorsModel)
        {
            var existingEntity = await _context.Sensors.FindAsync(sensorsModel.SensorId);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(sensorsModel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var sensorsModel = await _context.Sensors.FindAsync(id);
            if (sensorsModel != null)
            {
                _context.Sensors.Remove(sensorsModel);
                await _context.SaveChangesAsync();
            }
        }
    }
}

