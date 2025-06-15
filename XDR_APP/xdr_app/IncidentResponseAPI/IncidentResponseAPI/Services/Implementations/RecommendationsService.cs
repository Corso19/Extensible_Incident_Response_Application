using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;

namespace IncidentResponseAPI.Services.Implementations
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IRecommendationsRepository _recommendationsRepository;

        public RecommendationsService(IRecommendationsRepository recommendationsRepository)
        {
            _recommendationsRepository = recommendationsRepository;
        }

        public async Task<IEnumerable<RecommendationsDto>> GetAllAsync()
        {
            var recommendations = await _recommendationsRepository.GetAllAsync();
            return recommendations.Select(r => new RecommendationsDto
            {
                RecommendationId = r.RecommendationId,
                IncidentId = r.IncidentId,
                Description = r.Description,
                isCompleted = r.isCompleted
            }).ToList();
        }

        public async Task<RecommendationsDto> GetByIdAsync(int id)
        {
            var r = await _recommendationsRepository.GetByIdAsync(id);
            if (r == null) return null;

            return new RecommendationsDto
            {
                RecommendationId = r.RecommendationId,
                IncidentId = r.IncidentId,
                Description = r.Description,
                isCompleted = r.isCompleted
            };
        }

        public async Task AddAsync(RecommendationsDto recommendationsDto)
        {
            var recommendationsModel = new RecommendationsModel
            {
                IncidentId = recommendationsDto.IncidentId,
                Description = recommendationsDto.Description,
                isCompleted = recommendationsDto.isCompleted
            };

            await _recommendationsRepository.AddAsync(recommendationsModel);
        }

        public async Task UpdateAsync(int id, RecommendationsDto recommendationsDto)
        {
            var recommendationsModel = new RecommendationsModel
            {
                RecommendationId = id,
                IncidentId = recommendationsDto.IncidentId,
                Description = recommendationsDto.Description,
                isCompleted = recommendationsDto.isCompleted
            };

            await _recommendationsRepository.UpdateAsync(recommendationsModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _recommendationsRepository.DeleteAsync(id);
        }
    }
}
