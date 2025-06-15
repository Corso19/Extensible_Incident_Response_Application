using IncidentResponseAPI.Models;
using Microsoft.EntityFrameworkCore;
using IncidentResponseAPI.Repositories.Interfaces;

namespace IncidentResponseAPI.Repositories.Implementations
{
    public class RecommendationsRepository : IRecommendationsRepository
    {
        private readonly IncidentResponseContext _context;

        public RecommendationsRepository(IncidentResponseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecommendationsModel>> GetAllAsync()
        {
            return await _context.Recommendations.ToListAsync();
        }

        public async Task<RecommendationsModel> GetByIdAsync(int id)
        {
            return await _context.Recommendations.FindAsync(id);
        }

        public async Task AddAsync(RecommendationsModel recommendationsModel)
        {
            _context.Recommendations.Add(recommendationsModel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RecommendationsModel recommendationsModel)
        {
            _context.Entry(recommendationsModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recommendationsModel = await _context.Recommendations.FindAsync(id);
            if (recommendationsModel != null)
            {
                _context.Recommendations.Remove(recommendationsModel);
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<IEnumerable<RecommendationsModel>> GetByIncidentIdAsync(int incidentId)
        {
            return await _context.Recommendations
                .Where(r => r.IncidentId == incidentId)
                .ToListAsync();
        }
        
    }
}
