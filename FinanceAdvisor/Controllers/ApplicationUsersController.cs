using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly IApplicationUserService _service;

        public ApplicationUsersController(IApplicationUserService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationUserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAll()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApplicationUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApplicationUserDto>> GetById(Guid id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("deleted")]
        [ProducesResponseType(typeof(IEnumerable<ApplicationUserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ApplicationUserDto>>> GetAllDeleted()
        {
            var users = await _service.GetAllDeletedAsync();
            return Ok(users);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ApplicationUserDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            await _service.SoftDeleteAsync(id);
            return NoContent();
        }
    }
}
