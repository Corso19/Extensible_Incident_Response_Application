using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;

namespace IncidentResponseAPI.Services.Implementations;

public class EventsProcessingService : IEventsProcessingService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IIncidentDetectionService _incidentDetectionService;
    // private readonly ILogger<EventsProcessingService> _logger;

    public EventsProcessingService(IEventsRepository eventsRepository, IIncidentDetectionService incidentDetectionService)
    {
        _eventsRepository = eventsRepository;
        _incidentDetectionService = incidentDetectionService;
    }

    // public async Task ProcessEventsAsync(CancellationToken cancellationToken)
    // {
    //     var unprocessedEvents = await _eventsRepository.GetUnprocessedEventsAsync();
    //     
    //     foreach (var @event in unprocessedEvents)
    //     {
    //         //Forward to detection service
    //         await _incidentDetectionService.Detect(@event, cancellationToken);
    //         
    //         //Mark event as processed
    //         @event.isProcessed = true;
    //         await _eventsRepository.UpdateAsync(@event, cancellationToken);
    //     }
    // }
    
    public async Task ProcessEventsAsync(CancellationToken cancellationToken)
    {
        var unprocessedEvents = await _eventsRepository.GetUnprocessedEventsAsync(cancellationToken);

        foreach (var @event in unprocessedEvents)
        {
            // Forward to detection service
            await _incidentDetectionService.Detect(@event, cancellationToken);

            // Mark event as processed
            @event.isProcessed = true;
            await _eventsRepository.UpdateAsync(@event, cancellationToken);
        }
    }
}