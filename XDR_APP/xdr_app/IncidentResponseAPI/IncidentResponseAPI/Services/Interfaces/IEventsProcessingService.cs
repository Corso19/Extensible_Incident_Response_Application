namespace IncidentResponseAPI.Services.Interfaces;

public interface IEventsProcessingService
{
    Task ProcessEventsAsync(CancellationToken cancellationToken);
}