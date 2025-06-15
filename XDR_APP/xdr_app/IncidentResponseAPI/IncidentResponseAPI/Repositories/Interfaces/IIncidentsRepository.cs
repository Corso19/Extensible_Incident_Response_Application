using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Repositories.Interfaces
{
    public interface IIncidentsRepository
    {
        Task<IEnumerable<IncidentsModel>> GetAllAsync(bool includeEvent = false, bool includeRelations = false);
        Task<IncidentsModel> GetByIdAsync(int id);
        Task AddAsync(IncidentsModel incidentsModel, CancellationToken cancellationToken);
        Task UpdateAsync(IncidentsModel incidentsModel);
        Task DeleteAsync(int id);
    }
}
