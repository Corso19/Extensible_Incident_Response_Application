using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Services.Interfaces;

public interface ISensorHandler
{
    string SensorTypeKey { get; }
    Task<IEnumerable<EventsModel>> SyncEventsAsync(SensorsModel sensorsModel, CancellationToken cancellationToken);
}