using IncidentResponseAPI.Services.Interfaces;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Services.Implementations.Handlers
{
    public class EmailSensorHandler : ISensorHandler
    {
        private readonly IGraphAuthService _graphAuth;
        private readonly SecurityMetricsService _metrics;
        private readonly ILogger<EmailSensorHandler> _logger;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IEventsRepository _eventsRepository;

        public string SensorTypeKey => "MicrosoftEmail";

        public EmailSensorHandler(
            IGraphAuthService graphAuth,
            SecurityMetricsService metrics,
            IAttachmentRepository attachmentRepository,
            IEventsRepository eventsRepository,
            ILogger<EmailSensorHandler> logger)
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
                var messagesByUser = await _graphAuth.FetchEmailsForAllUsersAsync(
                    config.ClientSecret,
                    config.ApplicationId,
                    config.TenantId,
                    lastProcessedTime,
                    cancellationToken);

                foreach (var (userPrincipalName, messages) in messagesByUser)
                {
                    foreach (var message in messages)
                    {
                        // Check if message already exists
                        var existingEvent = await _eventsRepository.GetByMessageIdAsync(message.Id);
                        if (existingEvent != null)
                        {
                            _logger.LogInformation("Message {MessageId} already exists, skipping", message.Id);
                            continue;
                        }

                        var eventModel = await CreateEventFromMessage(message, sensor, userPrincipalName, config,
                            cancellationToken);
                        //await _eventsRepository.AddAsync(eventModel, cancellationToken);
                        events.Add(eventModel);

                        if (eventModel.Timestamp > maxProcessedTime)
                            maxProcessedTime = eventModel.Timestamp;
                    }
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
                _logger.LogError(ex, "Error syncing email events for sensor {SensorId}", sensor.SensorId);
                throw;
            }
        }

        private DateTime? GetLastProcessedTime(SensorsModel sensor)
        {
            if (string.IsNullOrEmpty(sensor.LastEventMarker))
                return null;

            var lastEventMarkerJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(sensor.LastEventMarker);
            return lastEventMarkerJson?.ContainsKey("LastProcessedTime") == true
                ? DateTime.Parse(lastEventMarkerJson["LastProcessedTime"])
                : null;
        }

        private async Task<EventsModel> CreateEventFromMessage(
            Message message,
            SensorsModel sensor,
            string userPrincipalName,
            Configuration config,
            CancellationToken cancellationToken)
        {
            var eventModel = new EventsModel
            {
                SensorId = sensor.SensorId,
                TypeName = SensorTypeKey,
                Subject = message.Subject ?? "No subject",
                Sender = message.Sender?.EmailAddress?.Address ?? "Unknown",
                Details = message.Body?.Content ?? "",
                Timestamp = message.ReceivedDateTime?.UtcDateTime.ToLocalTime() ?? DateTime.Now,
                isProcessed = false,
                MessageId = message.Id
            };
            
            await _eventsRepository.AddAsync(eventModel, cancellationToken);

            // Handle attachments
            var attachments = await _graphAuth.FetchAttachmentsAsync(
                config.ClientSecret,
                config.ApplicationId,
                config.TenantId,
                message.Id,
                userPrincipalName,
                cancellationToken);

            foreach (var attachment in attachments.OfType<FileAttachment>())
            {
                var attachmentModel = new AttachmentModel
                {
                    EventId = eventModel.EventId,
                    Name = attachment.Name ?? "Unnamed Attachment",
                    Size = attachment.Size ?? 0,
                    Content = attachment.ContentBytes
                };
                await _attachmentRepository.AddAttachmentAsync(attachmentModel, cancellationToken);
            }

            return eventModel;
        }
    }
}