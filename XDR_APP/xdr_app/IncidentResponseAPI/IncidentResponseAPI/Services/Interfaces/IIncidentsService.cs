using IncidentResponseAPI.Dtos;

namespace IncidentResponseAPI.Services.Interfaces
{
    public interface IIncidentsService
    {
        Task<IEnumerable<IncidentDto>> GetAllAsync();
        Task<IncidentDto> GetByIdAsync(int id);
        Task AddAsync(IncidentDto incidentDto, CancellationToken cancellationToken);
        Task UpdateAsync(int id, IncidentDto incidentDto);
        Task DeleteAsync(int id);
    }
}
