using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceAdvisor.API.Controllers
{
    [Authorize]
    //[Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]                  // All responses are JSON
    [Consumes("application/json")]                  // All requests are expected to be JSON
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ApiConventionType(typeof(DefaultApiConventions))] // Optional: standardizes Swagger conventions
    public class CreditConsultationCycleController : ControllerBase
    {
        private readonly ICreditConsultationCycleService _service;

        public CreditConsultationCycleController(ICreditConsultationCycleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all credit consultation cycles.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CreditConsultationCycleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CreditConsultationCycleDto>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get all credit consultation cycles for a specific client.
        /// </summary>
        /// <param name="clientId">The GUID of the client.</param>
        [Authorize(Policy = "UserOnly")]
        [HttpGet("client/{clientId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<CreditConsultationCycleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreditConsultationCycleDto>>> GetByClientId(Guid clientId)
        {
            if (clientId == Guid.Empty)
                return BadRequest("Client ID cannot be empty.");

            var result = await _service.GetAllByClientIdAsync(clientId);
            return Ok(result);
        }

        /// <summary>
        /// Get all credit consultation cycles assigned to a manager (advisor).
        /// </summary>
        /// <param name="advisorId">The GUID of the advisor.</param>
        [HttpGet("advisor/{advisorId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<CreditConsultationCycleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreditConsultationCycleDto>>> GetByManagerId(Guid advisorId)
        {
            if (advisorId == Guid.Empty)
                return BadRequest("Manager ID cannot be empty.");

            var result = await _service.GetAllByManagerIdAsync(advisorId);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cycle = await _service.GetByIdAsync(id);
            if (cycle == null)
                return NotFound();

            return Ok(cycle);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCreditConsultationCycleDto dto)
        {
            var createdCycle = await _service.CreateAsync(dto);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCreditConsultationCycleDto dto)
        {
            var updated = await _service.UpdateAsync(dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}/advisor/{advisorId:guid}")]
        public async Task<IActionResult> DeleteSelf(Guid id, Guid advisorId)
        {
            var deleted = await _service.DeleteSelfAsync(id, advisorId);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpGet("count/advisor/{advisorId:guid}")]
        public async Task<IActionResult> CountByAdvisor(Guid advisorId)
        {
            var count = await _service.CountByAdvisorAsync(advisorId);
            return Ok(count);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingCycles()
        {
            var pendingCycles = await _service.GetPendingCyclesAsync();
            return Ok(pendingCycles);
        }

    }
}
