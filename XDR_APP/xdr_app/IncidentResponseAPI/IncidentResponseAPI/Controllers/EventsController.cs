using IncidentResponseAPI.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using IncidentResponseAPI.Models;
using IncidentResponseAPI.Services.Interfaces;
using Newtonsoft.Json;

namespace IncidentResponseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventsService _eventsService;
        private readonly ISensorsService _sensorService;
        private readonly ILogger<EventsController> _logger;
        private readonly ISensorHandler _sensorHandler;

        public EventsController(
            IEventsService eventsService, 
            ISensorsService sensorsService, 
            ILogger<EventsController> logger,
            ISensorHandler sensorHandler)
        {
            _eventsService = eventsService;
            _sensorService = sensorsService;
            _logger = logger;
            _sensorHandler = sensorHandler;
        }

        // GET: api/Events
        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of events")]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
        {
            _logger.LogInformation("Fetching all events");

            try
            {
                var events = await _eventsService.GetAllEventsAsync();
                _logger.LogInformation("Successfuly fetched {Count} events", events?.Count() ?? 0);
                return Ok(events);
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Error occured while fetching all events.");
                return StatusCode(500, "An error occured while fetching events.");
            }
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets an event by ID")]
        public async Task<ActionResult<EventDto>> GetEventbyId(int id)
        {
            _logger.LogInformation("Fetching event with ID {Id}", id);

            try
            {
                var eventDto = await _eventsService.GetEventByIdAsync(id);
                if (eventDto == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", id);
                    return NotFound($"Event with ID {id} not found.");
                }
                _logger.LogInformation("Successfully fetched event with ID {Id}", id);
                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while fetching event with ID {Id}", id);
                return StatusCode(500, "An error occured while fetching the event.");
            }
        }

        // POST: api/Events
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new event")]
        public async Task<ActionResult<EventDto>> PostEvent(EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding a new event");

            try
            {
                await _eventsService.AddEventAsync(eventDto, cancellationToken);
                _logger.LogInformation("Successfully added a new event with ID {Id}", eventDto.EventId);
                return CreatedAtAction(nameof(GetEventbyId), new { id = eventDto.EventId }, eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while adding a new event.");
                return StatusCode(500, "An error occured while adding the event.");
            }
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing event")]
        public async Task<IActionResult> PutEvent(int id, EventDto eventDto, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating event with ID {Id}", id);
            
            if (id != eventDto.EventId)
            {
                _logger.LogWarning("Event ID in the ({RequesstId}) does not match the ({PayloadId})", id, eventDto.EventId);
                return BadRequest("Event ID mismatch");
            }

            try
            {
                await _eventsService.UpdateEventAsync(eventDto, cancellationToken);
                _logger.LogInformation("Successfully updated event with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while updating event with ID {Id}", id);
                return StatusCode(500, "An error occured while updating the event.");
            }
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes an event by ID")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            _logger.LogInformation("Deleting event with ID {EventId}", id);

            try
            {
                var eventDto = await _eventsService.GetEventByIdAsync(id);
                if (eventDto == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", id);
                    return NotFound($"Event with ID {id} not found.");
                }

                await _eventsService.DeleteEventAsync(id);
                _logger.LogInformation("Successfully deleted event with ID {EventId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while deleting event with ID {EventId}", id);
                return StatusCode(500, "An error occured while deleting the event.");
            }
        }
        
        // POST: api/Events/sync/{sensorId}
        // [HttpPost("sync/{sensorId}")]
        // [SwaggerOperation(Summary = "Syncs events (emails) for a specific sensor")]
        // public async Task<IActionResult> SyncEvents(int sensorId, CancellationToken cancellationToken)
        // {
        //     _logger.LogInformation("Syncing events for sensor {SensorId}", sensorId);

        //     try
        //     {
        //         await _eventsService.SyncEventsAsync(sensorId, cancellationToken);
        //         _logger.LogInformation("Events synced successfully for sensor {SensorId}", sensorId);
        //         return Ok("Events synced successfully.");
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error occurred while syncing events for sensor {SensorId}", sensorId);
        //         return StatusCode(500, "An error occurred while syncing events.");
        //     }
        // }

        
        // GET: api/Events/{eventId}/attachments
        [HttpGet("{eventId}/attachments")]
        [SwaggerOperation(Summary = "Gets attachments for a specific event")]
        public async Task<ActionResult<IEnumerable<AttachmentDto>>> GetAttachmentsByEventId(int eventId)
        {
            _logger.LogInformation("Fetching attachments for event with ID {EventId}", eventId);

            try
            {
                var attachments = await _eventsService.GetAttachmentsByEventIdAsync(eventId);
                if (attachments == null || !attachments.Any())
                {
                    _logger.LogWarning("No attachments found for event with ID {EventId}", eventId);
                    return NotFound("No attachments found for the given event ID.");
                }
                _logger.LogInformation("Successfully fetched {Count} attachments for event with ID {EventId}", attachments.Count(), eventId);
                return Ok(attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while fetching attachments for event with ID {EventId}", eventId);
                return StatusCode(500, "An error occured while fetching attachments.");
            }
        }
        
        [HttpGet("{sensorId}/message/{messageId}")]
        [SwaggerOperation(Summary = "Fetches the content of a specific email message")]
        public async Task<ActionResult<string>> GetMessageContent(int sensorId, string messageId)
        {
            _logger.LogInformation("Fetching content for message with ID {MessageId} for sensor {SensorId}", messageId, sensorId);

            try
            {
                // Fetch sensor using the SensorService
                var sensor = await _sensorService.GetByIdAsync(sensorId);
                if (sensor == null)
                {
                    _logger.LogWarning("Sensor with ID {SensorId} not found.", sensorId);
                    return NotFound("Sensor not found.");
                }

                // Deserialize the configuration JSON
                var config = JsonConvert.DeserializeObject<Configuration>(sensor.Configuration);
                if (config == null)
                {
                    _logger.LogWarning("Invalid configuration for sensor {SensorId}.", sensorId);
                    return BadRequest("Invalid configuration.");
                }

                // Fetch the message content using the events service
                var message = await _eventsService.FetchMessageContentAsync(
                    config.ClientSecret,
                    config.ApplicationId,
                    config.TenantId,
                    messageId
                );

                if (message == null)
                {
                    _logger.LogWarning("Message with ID {MessageId} not found for sensor {SensorId}", messageId, sensorId);
                    return NotFound("Message not found.");
                }

                _logger.LogInformation("Successfully fetched content for message with ID {MessageId} for sensor {SensorId}", messageId, sensorId);
                return Ok(message.Body.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching content for message with ID {MessageId} for sensor {SensorId}", messageId, sensorId);
                return StatusCode(500, "An error occurred while fetching message content.");
            }
        }


    }
}