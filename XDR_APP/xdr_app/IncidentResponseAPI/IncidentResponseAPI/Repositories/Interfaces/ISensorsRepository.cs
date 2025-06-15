using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Repositories.Interfaces
{
    public interface ISensorsRepository
    {
        Task<IEnumerable<SensorsModel>> GetAllAsync();
        Task<SensorsModel> GetByIdAsync(int id);
        Task<IEnumerable<SensorsModel>> GetAllEnabledAsync();
        Task AddAsync(SensorsModel sensorsModel);
        Task UpdateAsync(SensorsModel sensorsModel);
        Task DeleteAsync(int id);
    }
}

