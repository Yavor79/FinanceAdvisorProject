using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.API.Controllers
{
    [Authorize]
    //[Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationService _service;

        public ConsultationsController(IConsultationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetAll()
        {
            var consultations = await _service.GetAllAsync();
            return Ok(consultations);
        }

        [HttpGet("advisor/{advisorId:guid}/{consultationType}")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetByAdvisorId(Guid advisorId, string? consultationType)
        {
            var consultations = await _service.GetAllByAdvisorIdAsync(advisorId, consultationType);
            return Ok(consultations);
        }

        [HttpGet("client/{clientId:guid}/{consultationType}")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetByClientId(Guid clientId, string? consultationType)
        {
            var consultations = await _service.GetAllByClientIdAsync(clientId, consultationType);
            return Ok(consultations);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConsultationDto dto)
        {
            var createdConsultation = await _service.CreateAsync(dto);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateConsultationDto dto)
        {
            var updated = await _service.UpdateAsync(dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{consultationId:guid}/advisor/{advisorId:guid}")]
        public async Task<IActionResult> DeleteSelf(Guid consultationId, Guid advisorId)
        {
            var deleted = await _service.DeleteSelfAsync(consultationId, advisorId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var consultation = await _service.GetByIdAsync(id);
            if (consultation == null)
                return NotFound();

            return Ok(consultation);
        }

        [HttpPost("{consultationId:guid}/mark-completed/advisor/{advisorId:guid}")]
        public async Task<IActionResult> MarkAsCompleted(Guid consultationId, Guid advisorId)
        {
            var marked = await _service.MarkAsCompletedAsync(consultationId, advisorId);
            if (!marked)
                return NotFound();

            return NoContent();
        }

        [HttpGet("count/client/{clientId:guid}")]
        public async Task<IActionResult> CountByClientId(Guid clientId)
        {
            var count = await _service.CountByClientIdAsync(clientId);
            return Ok(count);
        }

        [HttpGet("upcoming/advisor/{advisorId:guid}")]
        public async Task<IActionResult> GetUpcomingForAdvisor(Guid advisorId, [FromQuery] int daysAhead = 7)
        {
            var upcomingConsultations = await _service.GetUpcomingForAdvisorAsync(advisorId, daysAhead);
            return Ok(upcomingConsultations);
        }
    }
}
