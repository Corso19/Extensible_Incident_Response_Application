using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IncidentResponseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly IIncidentsService _incidentsService;

        public IncidentsController(IIncidentsService incidentsService)
        {
            _incidentsService = incidentsService;
        }

        // GET: api/Incidents
        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of incidents")]
        public async Task<ActionResult<IEnumerable<IncidentDto>>> GetIncidents()
        {
            return Ok(await _incidentsService.GetAllAsync());
        }

        // GET: api/Incidents/5
        // [HttpGet("{id}")]
        // [SwaggerOperation(Summary = "Gets an incident by ID")]
        
        /// <summary>
        /// Gets an incident by its unique identifier
        /// </summary>
        /// <param name="id">The incident ID</param>
        /// <returns>The incident details</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets an incident by ID", 
            Description = "Retrieves detailed information about a specific incident")]
        [SwaggerResponse(StatusCodes.Status200OK, "The incident was found", typeof(IncidentDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The incident was not found")]
        public async Task<ActionResult<IncidentDto>> GetIncident(int id)
        {
            var incidentDto = await _incidentsService.GetByIdAsync(id);

            if (incidentDto == null)
            {
                return NotFound();
            }

            return incidentDto;
        }

        // POST: api/Incidents
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new incident")]
        public async Task<ActionResult<IncidentDto>> PostIncident(IncidentDto incidentDto, CancellationToken cancellationToken)
        {
            await _incidentsService.AddAsync(incidentDto, cancellationToken);
            return CreatedAtAction(nameof(GetIncident), new { id = incidentDto.IncidentId }, incidentDto);
        }

        // PUT: api/Incidents/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing incident")]
        public async Task<IActionResult> PutIncident(int id, IncidentDto incidentDto)
        {
            await _incidentsService.UpdateAsync(id, incidentDto);
            return NoContent();
        }

        // DELETE: api/Incidents/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes an incident by ID")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            await _incidentsService.DeleteAsync(id);
            return NoContent();
        }
    }
}
