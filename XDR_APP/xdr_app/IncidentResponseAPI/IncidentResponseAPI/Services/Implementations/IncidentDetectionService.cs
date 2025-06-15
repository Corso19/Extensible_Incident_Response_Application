using System.Security.Cryptography.Pkcs;
using IncidentResponseAPI.Constants;
using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Helpers;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace IncidentResponseAPI.Services.Implementations
{
    public class IncidentDetectionService : IIncidentDetectionService
    {
        private readonly IIncidentsRepository _incidentsRepository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<IncidentDetectionService> _logger;
        private readonly IRecommendationsRepository _recommendationsRepository;
        private readonly IHubContext<IncidentHub> _hubContext;
        private readonly SecurityMetricsService _metrics;

        public IncidentDetectionService(IHubContext<IncidentHub> hubContext, IIncidentsRepository incidentsRepository,
            IRecommendationsRepository recommendationsRepository, IEventsRepository eventsRepository,
            ILogger<IncidentDetectionService> logger,
            SecurityMetricsService metrics)
        {
            _incidentsRepository = incidentsRepository;
            _eventsRepository = eventsRepository;
            _logger = logger;
            _recommendationsRepository = recommendationsRepository;
            _hubContext = hubContext;
            _metrics = metrics;
        }

        public async Task<bool> Detect(EventsModel @event, CancellationToken cancellationToken)
        {
            var incidentsCreated = false;

            try
            {
                if (HasSuspiciousAttachment(@event))
                {
                    await CreateIncident(@event, IncidentType.SuspiciousAttachment, cancellationToken);
                    incidentsCreated = true;
                }

                if (IsExternalSender(@event))
                {
                    await CreateIncident(@event, IncidentType.ExternalSender, cancellationToken);
                    incidentsCreated = true;
                }

                if (await IsRepeatedEventPatternAsync(@event, cancellationToken))
                {
                    await CreateIncident(@event, IncidentType.RepeatedEventPattern, cancellationToken);
                    incidentsCreated = true;
                }

                if (await HasUnusualEmailVolumeAsync(@event, cancellationToken))
                {
                    await CreateIncident(@event, IncidentType.UnusualEmailVolume, cancellationToken);
                    incidentsCreated = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during incident detection.");
            }

            return incidentsCreated;
        }

        private bool HasSuspiciousAttachment(EventsModel @event)
        {
            return @event.Attachments.Any(a =>
                a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                a.Name.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) ||
                a.Name.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase) ||
                a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
        }

        private bool IsExternalSender(EventsModel @event)
        {
            var trustedDomains = new List<string> { "5smw12.onmicrosoft.com" };
            var senderDomain = @event.Sender?.Split('@').Last();
            return senderDomain != null && !trustedDomains.Contains(senderDomain);
        }

        private async Task<bool> IsRepeatedEventPatternAsync(EventsModel @event, CancellationToken cancellationToken)
        {
            var hasSameSubject = await HasSameSubjectPatternAsync(@event, cancellationToken);
            var hasTimeBasedPattern = await HasTimeBasedPatternAsync(@event, cancellationToken);

            return hasSameSubject || hasTimeBasedPattern;
        }

        private async Task<bool> HasSameSubjectPatternAsync(EventsModel @event, CancellationToken cancellationToken)
        {
            try
            {
                var recentEvents = await _eventsRepository.GetEventsBySubjectAsync(@event.Subject, cancellationToken);
                return recentEvents.Count() >= 5;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HasSameSubjectPatternAsync.");
                return false;
            }
        }

        private async Task<bool> HasTimeBasedPatternAsync(EventsModel @event, CancellationToken cancellationToken)
        {
            try
            {
                var matchingEvents =
                    await _eventsRepository.GetEventsByTimestampAsync(@event.Timestamp, cancellationToken);
                return matchingEvents.Count() >= 3;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HasTimeBasedPatternAsync.");
                return false;
            }
        }

        private async Task<bool> HasUnusualEmailVolumeAsync(EventsModel @event, CancellationToken cancellationToken)
        {
            try
            {
                var recentEvents = await _eventsRepository.GetEventsBySenderAsync(@event.Sender, cancellationToken);
                return recentEvents.Count() >= 10;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HasUnusualEmailVolumeAsync.");
                return false;
            }
        }

        private async Task CreateIncident(EventsModel @event, IncidentType incidentType,
            CancellationToken cancellationToken)
        {
            var (severity, description) = IncidentTypeMetadata.GetMetadata(incidentType);

            var incident = new IncidentsModel
            {
                Title = incidentType.ToString(),
                Description = description,
                Severity = severity,
                Type = incidentType,
                DetectedAt = DateTime.Now,
                Status = "Open",
                EventId = @event.EventId,
                Event = @event
            };

            try
            {
                // When detecting an incident
                // Convert severity to string and increment the incidents counter
                _metrics.IncidentsDetected.WithLabels(
                    severity.ToString(), // Convert int severity to string
                    incidentType.ToString() // Use the event's type name
                ).Inc();
                
                _metrics.IncidentsByType.WithLabels(
                    incidentType.ToString(),
                    "SensorType"// Use the incident type name)
                ).Inc();

                await _incidentsRepository.AddAsync(incident, cancellationToken);
                _logger.LogInformation("Incident created for event with ID {EventId}", @event.EventId);

                // Create multiple recommendations
                var recommendationSteps = RecommendationMetadata.GetRecommendations(incidentType);
                foreach (var step in recommendationSteps)
                {
                    var recommendation = new RecommendationsModel
                    {
                        IncidentId = incident.IncidentId,
                        Description = step,
                        isCompleted = false
                    };

                    await _recommendationsRepository.AddAsync(recommendation);
                    _logger.LogInformation("Recommendation created for incident with ID {IncidentId}",
                        incident.IncidentId);
                }

                var incidentDto = await MapToIncidentDto(incident, @event);
                _logger.LogInformation("Sending notification for incident with ID {IncidentId}", incident.IncidentId);
                await _hubContext.Clients.All.SendAsync("ReceivedIncident", incidentDto, cancellationToken);
                _logger.LogInformation("Notification sent for incident with ID {IncidentId}", incident.IncidentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident.");
                throw;
            }
        }

        private async Task<IncidentDto> MapToIncidentDto(IncidentsModel incident, EventsModel @event)
        {
            return new IncidentDto
            {
                IncidentId = incident.IncidentId,
                Title = incident.Title,
                Description = incident.Description,
                DetectedAt = incident.DetectedAt,
                Status = incident.Status,
                Type = incident.Type,
                Severity = incident.Severity,
                EventId = incident.EventId,
                Event = new EventDto
                {
                    EventId = @event.EventId,
                    TypeName = @event.TypeName,
                    Subject = @event.Subject,
                    Sender = @event.Sender,
                    Details = @event.Details,
                    Timestamp = @event.Timestamp,
                    Attachments = @event.Attachments.Select(a => new AttachmentDto
                    {
                        AttachmentId = a.AttachmentId,
                        Name = a.Name,
                        Size = a.Size,
                        Content = a.Content,
                        EventId = a.EventId
                    }).ToList()
                },
                Recommendations = RecommendationMetadata.GetRecommendations(incident.Type)
            };
        }
    }
}