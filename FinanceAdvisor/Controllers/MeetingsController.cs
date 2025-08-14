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
    public class MeetingsController : ControllerBase
    {
        private readonly IMeetingService _service;

        public MeetingsController(IMeetingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetAll()
        {
            var meetings = await _service.GetAllAsync();
            return Ok(meetings);
        }

        [HttpGet("cycle/{cycleId:guid}")]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetByCycleId(Guid cycleId)
        {
            var meetings = await _service.GetAllByCycleIdAsync(cycleId);
            return Ok(meetings);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var meeting = await _service.GetByIdAsync(id);
            if (meeting == null)
                return NotFound();

            return Ok(meeting);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeetingDto dto)
        {
            var createdMeeting = await _service.CreateAsync(dto);
            if (createdMeeting == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetById), new { id = createdMeeting.Id }, createdMeeting);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateMeetingDto dto)
        {
            var updated = await _service.UpdateAsync(dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}/owner/{cycleOwnerId:guid}")]
        public async Task<IActionResult> Delete(Guid id, Guid cycleOwnerId)
        {
            var deleted = await _service.DeleteAsync(id, cycleOwnerId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming([FromQuery] DateTime fromDate)
        {
            var meetings = await _service.GetUpcomingMeetingsAsync(fromDate);
            return Ok(meetings);
        }

        [HttpGet("count/cycle/{cycleId:guid}")]
        public async Task<IActionResult> CountByCycleId(Guid cycleId)
        {
            var count = await _service.CountByCycleIdAsync(cycleId);
            return Ok(count);
        }
    }
}
