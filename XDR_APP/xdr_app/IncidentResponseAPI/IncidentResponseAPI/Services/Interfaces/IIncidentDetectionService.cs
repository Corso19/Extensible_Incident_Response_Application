using IncidentResponseAPI.Models;

namespace IncidentResponseAPI.Services.Interfaces;

public interface IIncidentDetectionService
{
    Task<bool> Detect(EventsModel @event, CancellationToken cancellationToken);
}