using IncidentResponseAPI.Dtos;

namespace IncidentResponseAPI.Services.Interfaces
{
    public interface IRecommendationsService
    {
        Task<IEnumerable<RecommendationsDto>> GetAllAsync();
        Task<RecommendationsDto> GetByIdAsync(int id);
        Task AddAsync(RecommendationsDto recommendationsDto);
        Task UpdateAsync(int id, RecommendationsDto recommendationsDto);
        Task DeleteAsync(int id);
    }
}
