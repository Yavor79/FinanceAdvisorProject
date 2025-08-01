using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceAdvisor.API.Controllers
{
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
        /// <param name="managerId">The GUID of the advisor.</param>
        [HttpGet("manager/{managerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<CreditConsultationCycleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<CreditConsultationCycleDto>>> GetByManagerId(Guid managerId)
        {
            if (managerId == Guid.Empty)
                return BadRequest("Manager ID cannot be empty.");

            var result = await _service.GetAllByManagerIdAsync(managerId);
            return Ok(result);
        }
    }
}
