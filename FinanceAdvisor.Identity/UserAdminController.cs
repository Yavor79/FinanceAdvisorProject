using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Common.Logging;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceAdvisor.Identity
{
    //[Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    [ApiController]
    [Route("api/admin/users")]
    
    [EnableCors("AllowFinanceApp")]
    public class UserAdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserAdminController> _logger;

        public UserAdminController(UserManager<User> userManager, ILogger<UserAdminController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }


        [HttpPost("{id}/role")]
        public async Task<IActionResult> SetRole(Guid id, [FromBody] string role)
        {
            Console.WriteLine("SetRole hit!!!!");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok();
        }

        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetRoles(Guid id)
        {
            Console.WriteLine("GetRole hit!!!!");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }

        [HttpGet("usersOnly")]
        public async Task<IEnumerable<ApplicationUserDto>> GetAllWithRoleUserAsync()
        {

            var usersOnly = await _userManager.GetUsersInRoleAsync("User");
            
            var usersDto = usersOnly.Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsDeleted = u.IsDeleted
            });

            foreach (var dto in usersDto)
            {
                _logger.LogObjectProperties(dto, "[UserAdminController]");
            }
           

            return usersDto;
        }

        [HttpPost("create")]
        public async Task<bool> CreateAsync([FromBody]ApplicationUserDto dto)
        {
            _logger.LogObjectProperties(dto, "[UserAdminController]");
            

            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Invalid user data");
            var userExists = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower() ||
                u.UserName.ToLower() == dto.UserName.ToLower());

            if(userExists == null)
            {
                User user = new User
                {
                    //Id = dto.Id,
                    Id = Guid.NewGuid(),
                    UserName = dto.UserName,
                    CreatedAt = dto.CreatedAt,
                    IsDeleted = dto.IsDeleted,
                    Email = dto.Email
                };
                await _userManager.CreateAsync(user);
                var a = await _userManager.AddToRoleAsync(user, "User"); // by default
                return true;
            }

            return false;
        }
    }

}
