using IncidentResponseAPI.Constants;
using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Repositories.Interfaces;
using IncidentResponseAPI.Services.Interfaces;

namespace IncidentResponseAPI.Services.Implementations
{
    public class IncidentsService : IIncidentsService
    {
        private readonly IIncidentsRepository _incidentsRepository;

        public IncidentsService(IIncidentsRepository incidentsRepository)
        {
            _incidentsRepository = incidentsRepository;
        }

       public async Task<IEnumerable<IncidentDto>> GetAllAsync()
    {
        var incidents = await _incidentsRepository.GetAllAsync(includeEvent: true);
        return incidents.Select(i => new IncidentDto
        {
            IncidentId = i.IncidentId,
            Title = i.Title,
            Description = i.Description,
            DetectedAt = i.DetectedAt,
            Status = i.Status,
            Type = i.Type,
            Severity = i.Severity,
            EventId = i.EventId,
            Event = i.Event != null ? new EventDto
            {
                EventId = i.Event.EventId,
                TypeName = i.Event.TypeName,
                Subject = i.Event.Subject,
                Sender = i.Event.Sender,
                Details = i.Event.Details,
                Timestamp = i.Event.Timestamp,
                Attachments = i.Event.Attachments?.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    Name = a.Name,
                    Size = a.Size,
                    Content = a.Content,
                    EventId = a.EventId
                }).ToList()
            } : null,
            Recommendations = RecommendationMetadata.GetRecommendations(i.Type)
        }).ToList();
    }

    public async Task<IncidentDto> GetByIdAsync(int id)
    {
        var i = await _incidentsRepository.GetByIdAsync(id);
        if (i == null) return null;

        return new IncidentDto
        {
            IncidentId = i.IncidentId,
            Title = i.Title,
            Description = i.Description,
            DetectedAt = i.DetectedAt,
            Status = i.Status,
            Type = i.Type,
            Severity = i.Severity,
            EventId = i.EventId,
            Event = i.Event != null ? new EventDto
            {
                EventId = i.Event.EventId,
                TypeName = i.Event.TypeName,
                Subject = i.Event.Subject,
                Sender = i.Event.Sender,
                Details = i.Event.Details,
                Timestamp = i.Event.Timestamp,
                Attachments = i.Event.Attachments?.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    Name = a.Name,
                    Size = a.Size,
                    Content = a.Content,
                    EventId = a.EventId
                }).ToList()
            } : null,
            Recommendations = RecommendationMetadata.GetRecommendations(i.Type)
        };
    }
        public async Task AddAsync(IncidentDto incidentDto, CancellationToken cancellationToken)
        {
            var incidentsModel = new IncidentsModel
            {
                Title = incidentDto.Title,
                Description = incidentDto.Description,
                DetectedAt = incidentDto.DetectedAt,
                Status = incidentDto.Status,
                Type = incidentDto.Type,
                Severity = incidentDto.Severity,
                EventId = incidentDto.EventId
            };

            await _incidentsRepository.AddAsync(incidentsModel, cancellationToken);
        }

        public async Task UpdateAsync(int id, IncidentDto incidentDto)
        {
            var incidentsModel = new IncidentsModel
            {
                IncidentId = id,
                Title = incidentDto.Title,
                Description = incidentDto.Description,
                DetectedAt = incidentDto.DetectedAt,
                Status = incidentDto.Status
            };

            await _incidentsRepository.UpdateAsync(incidentsModel);
        }

        public async Task DeleteAsync(int id)
        {
            await _incidentsRepository.DeleteAsync(id);
        }
    }
}