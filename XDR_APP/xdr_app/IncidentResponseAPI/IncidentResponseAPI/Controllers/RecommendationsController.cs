using IncidentResponseAPI.Dtos;
using IncidentResponseAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IncidentResponseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationsService _recommendationsService;

        public RecommendationsController(IRecommendationsService recommendationsService)
        {
            _recommendationsService = recommendationsService;
        }

        // GET: api/Recommendations
        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of recommendations")]
        public async Task<ActionResult<IEnumerable<RecommendationsDto>>> GetRecommendations()
        {
            return Ok(await _recommendationsService.GetAllAsync());
        }

        // GET: api/Recommendations/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets a recommendation by ID")]
        public async Task<ActionResult<RecommendationsDto>> GetRecommendation(int id)
        {
            var recommendationDto = await _recommendationsService.GetByIdAsync(id);

            if (recommendationDto == null)
            {
                return NotFound();
            }

            return recommendationDto;
        }

        // POST: api/Recommendations
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new recommendation")]
        public async Task<ActionResult<RecommendationsDto>> PostRecommendation(RecommendationsDto recommendationsDto)
        {
            await _recommendationsService.AddAsync(recommendationsDto);
            return CreatedAtAction(nameof(GetRecommendation), new { id = recommendationsDto.RecommendationId }, recommendationsDto);
        }

        // PUT: api/Recommendations/5
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing recommendation")]
        public async Task<IActionResult> PutRecommendation(int id, RecommendationsDto recommendationsDto)
        {
            await _recommendationsService.UpdateAsync(id, recommendationsDto);
            return NoContent();
        }

        // DELETE: api/Recommendations/5
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a recommendation by ID")]
        public async Task<IActionResult> DeleteRecommendation(int id)
        {
            await _recommendationsService.DeleteAsync(id);
            return NoContent();
        }
    }
}
