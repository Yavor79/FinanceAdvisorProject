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

        [HttpGet("advisor/{advisorId:guid}")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetByAdvisorId(Guid advisorId)
        {
            var consultations = await _service.GetAllByAdvisorIdAsync(advisorId);
            return Ok(consultations);
        }

        [HttpGet("client/{clientId:guid}")]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetByClientId(Guid clientId)
        {
            var consultations = await _service.GetAllByClientIdAsync(clientId);
            return Ok(consultations);
        }
    }
}
