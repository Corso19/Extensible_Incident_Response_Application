using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Models;


namespace IncidentResponseAPI.Services.Interfaces;
public interface ISensorsService
{
    Task<IEnumerable<SensorDto>> GetAllAsync();
    Task RunSensorAsync(SensorsModel sensor, CancellationToken cancellationToken);
    Task <IEnumerable<SensorsModel>> GetAllEnabledAsync();
    Task<SensorDto> GetByIdAsync(int id);
    Task<SensorDto> AddAsync(SensorDto sensorDto);
    Task UpdateAsync(int id, SensorDto sensorDto);
    Task DeleteAsync(int id);
    Task SetEnabledAsync(int id);
    //void CancelAllSensors();

}