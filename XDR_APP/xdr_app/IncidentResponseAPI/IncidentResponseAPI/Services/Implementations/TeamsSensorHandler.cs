using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Services.Implementations;

public class TeamsSensorHandler : ISensorHandler
{
    private IGraphAuthService _graphAuth;
    private SecurityMetricsService _metrics;
    private readonly ILogger<TeamsSensorHandler> _logger;
    private readonly IEventsRepository _eventsRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    
    public string SensorTypeKey => "MicrosoftTeams";
    
    public TeamsSensorHandler(
        IGraphAuthService graphAuth,
        SecurityMetricsService metrics,
        IAttachmentRepository attachmentRepository,
        IEventsRepository eventsRepository,
        ILogger<TeamsSensorHandler> logger)
    {
        _graphAuth = graphAuth;
        _metrics = metrics;
        _attachmentRepository = attachmentRepository;
        _eventsRepository = eventsRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<EventsModel>> SyncEventsAsync(
        SensorsModel sensor,
        CancellationToken cancellationToken)
    {
        _metrics.SensorRuns.WithLabels(SensorTypeKey, sensor.SensorName).Inc();
        
        try
        {
            var config = JsonConvert.DeserializeObject<Configuration>(sensor.Configuration);
            DateTime? lastProcessedTime = GetLastProcessedTime(sensor);
            var maxProcessedTime = lastProcessedTime ?? DateTime.MinValue;

            var events = new List<EventsModel>();
            
            // Fetch Teams messages since last processed time
            var teamMessages = await _graphAuth.FetchTeamsMessagesAsync(
                config.ClientSecret,
                config.ApplicationId,
                config.TenantId,
                lastProcessedTime,
                cancellationToken);

            foreach (var message in teamMessages)
            {
                // Check if message already exists
                var existingEvent = await _eventsRepository.GetByMessageIdAsync(message.Id);
                if (existingEvent != null)
                {
                    _logger.LogInformation("Teams message {MessageId} already exists, skipping", message.Id);
                    continue;
                }

                var eventModel = await CreateEventFromTeamsMessage(message, sensor, config, cancellationToken);
                events.Add(eventModel);

                if (eventModel.Timestamp > maxProcessedTime)
                    maxProcessedTime = eventModel.Timestamp;
            }

            // Update LastEventMarker
            sensor.LastEventMarker = JsonConvert.SerializeObject(new
            {
                LastProcessedTime = maxProcessedTime.ToString("o")
            });

            return events;
        }
        catch (Exception ex)
        {
            _metrics.SensorErrors.WithLabels(SensorTypeKey, sensor.SensorName).Inc();
            _logger.LogError(ex, "Error syncing Teams events for sensor {SensorId}", sensor.SensorId);
            throw;
        }
    }
    
    private DateTime? GetLastProcessedTime(SensorsModel sensor)
    {
        // Same implementation as in EmailSensorHandler
        if (string.IsNullOrEmpty(sensor.LastEventMarker))
            return null;

        var lastEventMarkerJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(sensor.LastEventMarker);
        return lastEventMarkerJson?.ContainsKey("LastProcessedTime") == true
            ? DateTime.Parse(lastEventMarkerJson["LastProcessedTime"])
            : null;
    }
    
    private async Task<EventsModel> CreateEventFromTeamsMessage(
        ChatMessage message, 
        SensorsModel sensor,
        Configuration config,
        CancellationToken cancellationToken)
    {
        var eventModel = new EventsModel
        {
            SensorId = sensor.SensorId,
            TypeName = SensorTypeKey,
            Subject = "Teams Message",  // Teams messages don't have subjects
            Sender = message.From?.User?.DisplayName ?? "Unknown",
            Details = message.Body?.Content ?? "",
            Timestamp = message.CreatedDateTime?.LocalDateTime ?? DateTime.Now,
            isProcessed = false,
            MessageId = message.Id
        };

        // Save event first to get valid ID
        await _eventsRepository.AddAsync(eventModel, cancellationToken);

        // Process attachments if any
        if (message.Attachments != null && message.Attachments.Any())
        {
            foreach (var attachment in message.Attachments)
            {
                // For Teams, you'll need to download the attachment content separately
                var attachmentContent = await _graphAuth.FetchTeamsAttachmentAsync(
                    config.ClientSecret,
                    config.ApplicationId,
                    config.TenantId,
                    message.Id,
                    attachment.Id,
                    cancellationToken);

                var attachmentModel = new AttachmentModel
                {
                    EventId = eventModel.EventId,
                    Name = attachment.Name ?? "Unnamed Attachment",
                    Size = 0, // You might need to determine this from the content
                    Content = attachmentContent
                };
                await _attachmentRepository.AddAttachmentAsync(attachmentModel, cancellationToken);
            }
        }

        return eventModel;
    }
    
}