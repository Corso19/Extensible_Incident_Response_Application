using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.Graph.Models;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Services.Implementations.Handlers
{
    public class SharePointSensorHandler : ISensorHandler
    {
        private readonly IGraphAuthService _graphAuth;
        private readonly SecurityMetricsService _metrics;
        private readonly ILogger<SharePointSensorHandler> _logger;
        private readonly IEventsRepository _eventsRepository;
        private readonly IAttachmentRepository _attachmentRepository;

        public string SensorTypeKey => "MicrosoftSharePoint";

        public SharePointSensorHandler(
            IGraphAuthService graphAuth,
            SecurityMetricsService metrics,
            IAttachmentRepository attachmentRepository,
            IEventsRepository eventsRepository,
            ILogger<SharePointSensorHandler> logger)
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
                
                // Fetch SharePoint activities since last processed time
                var sharePointItems = await _graphAuth.FetchSharePointActivitiesAsync(
                    config.ClientSecret, 
                    config.ApplicationId,
                    config.TenantId,
                    lastProcessedTime,
                    cancellationToken);

                foreach (var item in sharePointItems)
                {
                    // Check if activity already exists
                    var existingEvent = await _eventsRepository.GetByMessageIdAsync(item.Id);
                    if (existingEvent != null)
                    {
                        _logger.LogInformation("SharePoint activity {ItemId} already exists, skipping", item.Id);
                        continue;
                    }

                    var eventModel = await CreateEventFromSharePointItem(item, sensor, config, cancellationToken);
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
                _logger.LogError(ex, "Error syncing SharePoint events for sensor {SensorId}", sensor.SensorId);
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

        private async Task<EventsModel> CreateEventFromSharePointItem(
            DriveItem item,
            SensorsModel sensor,
            Configuration config,
            CancellationToken cancellationToken)
        {
            var eventModel = new EventsModel
            {
                SensorId = sensor.SensorId,
                TypeName = SensorTypeKey,
                Subject = $"SharePoint Activity: {item.Name}",
                Sender = item.LastModifiedBy?.User?.DisplayName ?? "Unknown",
                Details = $"File: {item.Name}\nLocation: {item.ParentReference?.Path ?? "Unknown"}\nAction: {(item.Deleted != null ? "Deleted" : "Modified")}",
                Timestamp = item.LastModifiedDateTime?.LocalDateTime ?? DateTime.Now,
                isProcessed = false,
                MessageId = item.Id
            };

            await _eventsRepository.AddAsync(eventModel, cancellationToken);

            // For suspicious files, we might want to download and store as attachment for analysis
            if (IsSuspiciousFile(item))
            {
                try
                {
                    var fileContent = await _graphAuth.FetchSharePointFileContentAsync(
                        config.ClientSecret,
                        config.ApplicationId,
                        config.TenantId,
                        item.Id,
                        cancellationToken);

                    var attachmentModel = new AttachmentModel
                    {
                        EventId = eventModel.EventId,
                        Name = item.Name,
                        Size = (int)(item.Size ?? 0),
                        Content = fileContent
                    };
                    await _attachmentRepository.AddAttachmentAsync(attachmentModel, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to download suspicious file {ItemId}", item.Id);
                }
            }

            return eventModel;
        }

        private bool IsSuspiciousFile(DriveItem item)
        {
            if (item.Name == null) return false;
            
            // Check for potentially suspicious file extensions
            string[] suspiciousExtensions = { ".exe", ".bat", ".ps1", ".vbs", ".js", ".hta" };
            return suspiciousExtensions.Any(ext => item.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}