using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Infrastructure.Seed.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.API.Controllers
{
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
    }
}
