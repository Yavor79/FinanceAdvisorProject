using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.API.Controllers
{
    [Authorize]
    
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AdvisorsController : ControllerBase
    {
        private readonly IAdvisorService _service;

        public AdvisorsController(IAdvisorService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AdvisorDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AdvisorDto>>> GetAll()
        {
            var advisors = await _service.GetAllAsync();
            return Ok(advisors);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AdvisorDto>> GetById(Guid id)
        {
            var advisor = await _service.GetByIdAsync(id);
            if (advisor == null) return NotFound();
            return Ok(advisor);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<AdvisorDto>> GetByUserId(Guid userId)
        {
            var advisor = await _service.GetByUserIdAsync(userId);
            if (advisor == null) return NotFound();
            return Ok(advisor);
        }

        [HttpGet("specialization/{specialization}")]
        public async Task<ActionResult<IEnumerable<AdvisorDto>>> GetBySpecialization(Specialization specialization)
        {
            var advisors = await _service.GetBySpecializationAsync(specialization);
            return Ok(advisors);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AdvisorDto dto)
        {
            var result = await _service.CreateAsync(dto);
            if(result)
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Advisor already exists!");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AdvisorDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.SoftDeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("restore/{id:guid}")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var result = await _service.RestoreAsync(id);
            Console.WriteLine($"Result////////////////{result}");
            return NoContent();
        }
    }
}
