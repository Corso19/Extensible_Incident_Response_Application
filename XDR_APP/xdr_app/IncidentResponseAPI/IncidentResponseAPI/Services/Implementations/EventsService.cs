using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.Graph.Models;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Kiota.Abstractions;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Services.Implementations
{
    public class EventsService : IEventsService
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly GraphAuthProvider _graphAuthProvider;
        private readonly IGraphAuthService _graphAuthService;
        private readonly ILogger<EventsService> _logger;
        private readonly ISensorsRepository _sensorsRepository;
        private readonly IEventsProcessingService _eventsProcessingService;

        public EventsService(
            IEventsRepository eventsRepository,
            IAttachmentRepository attachmentRepository,
            IGraphAuthService graphAuthService,
            ILogger<EventsService> logger,
            ISensorsRepository sensorsRepository,
            IEventsProcessingService eventsProcessingService)
        {
            _eventsRepository = eventsRepository;
            _attachmentRepository = attachmentRepository;
            _graphAuthService = graphAuthService;
            _logger = logger;
            _sensorsRepository = sensorsRepository;
            _eventsProcessingService = eventsProcessingService;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            _logger.LogInformation("Fetching all events");
            try
            {
                var events = await _eventsRepository.GetAllAsync();

                var eventDtos = new List<EventDto>();
                foreach (var e in events)
                {
                    var attachments = await _attachmentRepository.GetAttachmentsByEventIdAsync(e.EventId);
                    eventDtos.Add(MapToDto(e, attachments));
                }

                _logger.LogInformation("Successfully fetched {Count} events", eventDtos.Count);
                return eventDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all events");
                throw;
            }
        }

        public async Task<EventDto> GetEventByIdAsync(int eventId)
        {
            _logger.LogInformation("Fetching event with ID {EventId}", eventId);
            try
            {
                var eventModel = await _eventsRepository.GetByIdAsync(eventId);
                if (eventModel == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", eventId);
                    return null;
                }

                var attachments = await _attachmentRepository.GetAttachmentsByEventIdAsync(eventId);
                _logger.LogInformation("Successfully fetched event with ID {EventId}", eventId);
                return MapToDto(eventModel, attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching event with ID {EventId}", eventId);
                throw;
            }
        }

        public async Task AddEventAsync(EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding new event with Subject: {Subject}", eventDto.Subject);
            try
            {
                var eventModel = MapToModel(eventDto);
                await _eventsRepository.AddAsync(eventModel, cancellationToken);

                if (eventDto.Attachments != null && eventDto.Attachments.Any())
                {
                    foreach (var attachment in eventDto.Attachments)
                    {
                        var attachmentModel = new AttachmentModel
                        {
                            EventId = eventModel.EventId,
                            Name = attachment.Name,
                            Size = attachment.Size,
                            Content = attachment.Content
                        };

                        await _attachmentRepository.AddAttachmentAsync(attachmentModel, cancellationToken);
                    }
                }

                //Trigger immediate detection
                await _eventsProcessingService.ProcessEventsAsync(cancellationToken);
                _logger.LogInformation("Successfully added event with Subject: {Subject}", eventDto.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding event with Subject: {Subject}", eventDto.Subject);
                throw;
            }
        }

        public async Task UpdateEventAsync(EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating event with ID {EventId}", eventDto.EventId);
            try
            {
                var eventModel = MapToModel(eventDto);
                await _eventsRepository.UpdateAsync(eventModel, cancellationToken);

                _logger.LogInformation("Successfully updated event with ID {EventId}", eventDto.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating event with ID {EventId}", eventDto.EventId);
                throw;
            }
        }

        public async Task DeleteEventAsync(int eventId)
        {
            _logger.LogInformation("Deleting event with ID {EventId}", eventId);
            try
            {
                var attachments = await _attachmentRepository.GetAttachmentsByEventIdAsync(eventId);
                foreach (var attachment in attachments)
                {
                    await _attachmentRepository.DeleteAttachmentAsync(attachment.AttachmentId);
                }

                await _eventsRepository.DeleteAsync(eventId);
                _logger.LogInformation("Successfully deleted event with ID {EventId}", eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting event with ID {EventId}", eventId);
                throw;
            }
        }

        // public async Task SyncEventsAsync(int sensorId, CancellationToken cancellationToken)
        // {
        //     _logger.LogInformation("Starting sync for sensor: {SensorId}", sensorId);
        //
        //     try
        //     {
        //         var sensor = await _sensorsRepository.GetByIdAsync(sensorId);
        //         if (sensor == null)
        //         {
        //             _logger.LogWarning("Sensor with ID {SensorId} not found.", sensorId);
        //             return;
        //         }
        //
        //         var config = JsonConvert.DeserializeObject<Configuration>(sensor.Configuration);
        //         if (config == null || string.IsNullOrEmpty(config.ClientSecret) ||
        //             string.IsNullOrEmpty(config.ApplicationId) || string.IsNullOrEmpty(config.TenantId))
        //         {
        //             _logger.LogWarning("Invalid configuration for sensor {SensorId}.", sensorId);
        //             return;
        //         }
        //
        //         DateTime? lastProcessedTime = null;
        //         if (!string.IsNullOrEmpty(sensor.LastEventMarker))
        //         {
        //             var lastEventMarkerJson =
        //                 JsonConvert.DeserializeObject<Dictionary<string, string>>(sensor.LastEventMarker);
        //             if (lastEventMarkerJson != null && lastEventMarkerJson.ContainsKey("LastProcessedTime"))
        //             {
        //                 lastProcessedTime = DateTime.Parse(lastEventMarkerJson["LastProcessedTime"]);
        //             }
        //         }
        //
        //         DateTime maxProcessedTime = lastProcessedTime ?? DateTime.MinValue;
        //
        //         var messagesByUser = await _graphAuthService.FetchEmailsForAllUsersAsync(
        //             config.ClientSecret,
        //             config.ApplicationId,
        //             config.TenantId,
        //             lastProcessedTime,
        //             cancellationToken);
        //
        //         int newEmailsCount = 0;
        //
        //         TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        //
        //         foreach (var (userPrincipalName, messages) in messagesByUser)
        //         {
        //             _logger.LogInformation("Processing messages for user: {UserPrincipalName}", userPrincipalName);
        //             foreach (var message in messages)
        //             {
        //                 var eventModel = new EventsModel
        //                 {
        //                     SensorId = sensor.SensorId,
        //                     TypeName = "Email",
        //                     Subject = message.Subject ?? "No subject",
        //                     Sender = message.Sender?.EmailAddress?.Address ?? "Unknown Sender",
        //                     Details = message.Body?.Content ?? "No Content",
        //                     Timestamp = TimeZoneInfo.ConvertTimeFromUtc(
        //                         message.ReceivedDateTime?.UtcDateTime ?? DateTime.UtcNow, localTimeZone),
        //                     isProcessed = false,
        //                     MessageId = message.Id
        //                 };
        //
        //                 try
        //                 {
        //                     await _eventsRepository.AddAsync(eventModel, cancellationToken);
        //                 }
        //                 catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx &&
        //                                                    sqlEx.Number == 2627)
        //                 {
        //                     _logger.LogInformation("Message with ID {MessageId} already exists. Skipping.", message.Id);
        //                     continue;
        //                 }
        //
        //                 _logger.LogInformation("Processing message from sender: {Sender}, Subject: {Subject}",
        //                     message.Sender?.EmailAddress?.Address ?? "Unknown Sender",
        //                     message.Subject ?? "No subject");
        //
        //                 if (eventModel.Timestamp > maxProcessedTime)
        //                 {
        //                     _logger.LogInformation("Updating MaxProcessedTime from {Old} to {New}",
        //                         maxProcessedTime, eventModel.Timestamp);
        //                     maxProcessedTime = eventModel.Timestamp;
        //                 }
        //
        //                 newEmailsCount++;
        //
        //                 var attachments = await _graphAuthService.FetchAttachmentsAsync(
        //                     config.ClientSecret,
        //                     config.ApplicationId,
        //                     config.TenantId,
        //                     message.Id,
        //                     userPrincipalName,
        //                     cancellationToken);
        //
        //                 foreach (var attachment in attachments.OfType<FileAttachment>())
        //                 {
        //                     var attachmentModel = new AttachmentModel
        //                     {
        //                         Name = attachment.Name ?? "Unnamed Attachment",
        //                         Size = attachment.Size ?? 0,
        //                         Content = attachment.ContentBytes,
        //                         EventId = eventModel.EventId
        //                     };
        //
        //                     await _attachmentRepository.AddAttachmentAsync(attachmentModel, cancellationToken);
        //                 }
        //             }
        //         }
        //
        //         _logger.LogInformation("Sync completed for sensor: {SensorId}. {NewEmailsCount} new emails were added.",
        //             sensorId, newEmailsCount);
        //         _logger.LogInformation("Final MaxProcessedTime: {MaxProcessedTime}", maxProcessedTime);
        //
        //         if (newEmailsCount > 0)
        //         {
        //             var updatedLastEventMarker = new Dictionary<string, string>
        //             {
        //                 { "LastProcessedTime", maxProcessedTime.ToString("o") }
        //             };
        //             _logger.LogInformation("Updating LastEventMarker to: {Marker}",
        //                 JsonConvert.SerializeObject(updatedLastEventMarker));
        //
        //             sensor.LastEventMarker = JsonConvert.SerializeObject(updatedLastEventMarker);
        //             await _sensorsRepository.UpdateAsync(sensor);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error occurred while syncing events for sensor {SensorId}", sensorId);
        //         throw;
        //     }
        // }

        public async Task<Message> FetchMessageContentAsync(string clientSecret, string applicationId, string tenantId,
            string messageId)
        {
            _logger.LogInformation("Fetching message content for message ID: {MessageId}", messageId);

            try
            {
                // Use the graphAuthService to handle authentication
                var graphClient =
                    await _graphAuthService.GetAuthenticatedGraphClient(clientSecret, applicationId, tenantId);

                // Fetch the message content
                return await graphClient.Me.Messages[messageId].GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching message content for message ID: {MessageId}",
                    messageId);
                throw;
            }
        }


        public async Task<IEnumerable<AttachmentDto>> GetAttachmentsByEventIdAsync(int eventId)
        {
            _logger.LogInformation("Fetching attachments for event ID: {EventId}", eventId);
            try
            {
                var attachments = await _attachmentRepository.GetAttachmentsByEventIdAsync(eventId);
                _logger.LogInformation("Successfully fetched {Count} attachments for event ID: {EventId}",
                    attachments.Count(), eventId);
                return attachments.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    Name = a.Name,
                    Size = a.Size,
                    Content = a.Content,
                    EventId = a.EventId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching attachments for event ID: {EventId}", eventId);
                throw;
            }
        }

        private EventDto MapToDto(EventsModel eventModel, IEnumerable<AttachmentModel> attachments)
        {
            return new EventDto
            {
                EventId = eventModel.EventId,
                SensorId = eventModel.SensorId,
                TypeName = eventModel.TypeName,
                Subject = eventModel.Subject,
                Sender = eventModel.Sender,
                Details = eventModel.Details,
                Timestamp = eventModel.Timestamp,
                isProcessed = eventModel.isProcessed,
                MessageId = eventModel.MessageId,
                Attachments = attachments.Select(attachment => new AttachmentDto
                {
                    AttachmentId = attachment.AttachmentId,
                    EventId = attachment.EventId,
                    Name = attachment.Name,
                    Size = attachment.Size,
                    Content = attachment.Content
                }).ToList()
            };
        }

        private EventsModel MapToModel(EventDto eventDto)
        {
            return new EventsModel
            {
                EventId = eventDto.EventId,
                SensorId = eventDto.SensorId,
                TypeName = eventDto.TypeName,
                Subject = eventDto.Subject,
                Sender = eventDto.Sender,
                Details = eventDto.Details,
                Timestamp = eventDto.Timestamp,
                isProcessed = eventDto.isProcessed,
                MessageId = eventDto.MessageId,
                Attachments = eventDto.Attachments.Select(a => new AttachmentModel
                {
                    AttachmentId = a.AttachmentId,
                    EventId = a.EventId,
                    Name = a.Name,
                    Size = a.Size,
                    Content = a.Content
                }).ToList()
            };
        }
    }
}