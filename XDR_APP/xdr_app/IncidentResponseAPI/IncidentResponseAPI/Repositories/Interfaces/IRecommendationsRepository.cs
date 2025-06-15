using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Repositories.Interfaces
{
    public interface IRecommendationsRepository
    {
        Task<IEnumerable<RecommendationsModel>> GetAllAsync();
        Task<RecommendationsModel> GetByIdAsync(int id);
        Task AddAsync(RecommendationsModel recommendationsModel);
        Task UpdateAsync(RecommendationsModel recommendationsModel);
        Task DeleteAsync(int id);
        Task<IEnumerable<RecommendationsModel>> GetByIncidentIdAsync(int incidentId);
    }
}
