using System.ComponentModel.DataAnnotations;
using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Orchestrators;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IncidentResponseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorsService _sensorsService;
        private readonly ILogger<SensorsController> _logger;
        private readonly SensorsOrchestrator _sensorsOrchestrator;

        public SensorsController(ISensorsService sensorsService, ILogger<SensorsController> logger, SensorsOrchestrator sensorsOrchestrator)
        {
            _sensorsService = sensorsService;
            _logger = logger;
            _sensorsOrchestrator = sensorsOrchestrator;
        }

        // GET: api/Sensors
        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of sensors")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetAllSensors()
        {
            _logger.LogInformation("Fetching all sensors");

            try
            {
                var sensors = await _sensorsService.GetAllAsync();
                _logger.LogInformation("Successfully fetched {Count} sensors", sensors?.Count() ?? 0);
                return Ok(sensors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all sensors");
                return StatusCode(500, "An error occurred while fetching sensors.");
            }
        }
        
        // GET: api/Sensors/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets a sensor by ID")]
        public async Task<ActionResult<SensorDto>> GetSensorById(int id)
        {
            _logger.LogInformation("Fetching sensor with ID {Id}", id);

            try
            {
                var sensor = await _sensorsService.GetByIdAsync(id);
                if (sensor == null)
                {
                    _logger.LogWarning("Sensor with ID {Id} not found", id);
                    return NotFound("Sensor not found.");
                }
                _logger.LogInformation("Successfully fetched sensor with ID {Id}", id);
                return Ok(sensor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching sensor with ID {Id}", id);
                return StatusCode(500, "An error occurred while fetching the sensor.");
            }
        }
        
        // POST: api/Sensors
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new sensor")]
        public async Task<ActionResult<SensorDto>> PostSensor(SensorDto sensorDto)
        {
            _logger.LogInformation("Creating a new sensor");

            try
            {
                var createdSensor =  await _sensorsService.AddAsync(sensorDto);
                _logger.LogInformation("Successfully created sensor with ID {Id}", sensorDto.SensorId);
                return CreatedAtAction(nameof(GetSensorById), new { id = createdSensor.SensorId }, createdSensor);
                
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error occured while creating the new sensor (configuration)");
                return BadRequest("Validation error regarding config occured while creating the sensor.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new sensor");
                return StatusCode(500, "An error occurred while creating the sensor.");
            }
        }
        
        [HttpPost("start-orchestrator")]
        [SwaggerOperation(Summary = "Starts the orchestrator to process all enabled sensors")]
        public async Task<IActionResult> StartOrchestrator()
        {
            _logger.LogInformation("Starting the orchestrator to process all enabled sensors");

            try
            {
                _sensorsOrchestrator.IsRunning = true;
                
                var enabledSensors = await _sensorsService.GetAllEnabledAsync();
                foreach (var sensor in enabledSensors)
                {
                    _sensorsOrchestrator.EnqueueSensor(sensor);
                }
                _logger.LogInformation("Successfully Enqueued all enabled sensors to the orchestrator");
                return Ok("Orchestrator started successfully.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while starting the orchestrator");
                return StatusCode(500, "An error occurred while starting the orchestrator.");
            }
        }
        
        [HttpGet("orchestrator-status")]
        [SwaggerOperation(Summary = "Gets the status of the orchestrator")]
        public IActionResult GetOrchestratorStatus()
        {
            return Ok(new { IsRunning = _sensorsOrchestrator.IsRunning });
        }

        // PUT: api/Sensors/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing sensor")]
        public async Task<IActionResult> PutSensor(int id, SensorDto sensorDto)
        {
            _logger.LogInformation("Updating sensor with ID {Id}", id);

            try
            {
                // Use the id parameter instead of sensorDto.SensorId
                sensorDto.SensorId = id;
                await _sensorsService.UpdateAsync(id, sensorDto);
                _logger.LogInformation("Successfully updated sensor with ID {Id}", id);
                return Ok("Sensor updated successfully.");
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failed for sensor with ID {Id}", id);
                return BadRequest("Validation error occurred while updating the sensor.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating sensor with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the sensor.");
            }
        }

        // DELETE: api/Sensors/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a sensor by ID")]
        public async Task<IActionResult> DeleteSensor(int id)
        {
            _logger.LogInformation("Deleting sensor with ID {Id}", id);

            try
            {
                await _sensorsService.DeleteAsync(id);
                _logger.LogInformation("Successfully deleted sensor with ID {Id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting sensor with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the sensor.");
            }
        }
        
        // PUT: api/Sensors/{id}/set-enabled
        [HttpPut("{id}/set-enabled")]
        [SwaggerOperation(Summary = "Sets a sensor's enabled status by ID")]
        public async Task<IActionResult> SetEnabled(int id)
        {
            _logger.LogInformation("Toggling enabled status for sensor with ID {Id}", id);

            try
            {
                await _sensorsService.SetEnabledAsync(id);
                _logger.LogInformation("Successfully toggled enabled status for sensor with ID {Id}", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling enabled status for sensor with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the sensor's enabled status.");
            }
        }
    }
}