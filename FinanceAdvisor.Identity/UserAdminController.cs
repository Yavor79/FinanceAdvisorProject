using FinanceAdvisor.Application.DTOs;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Identity
{
    //[Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminOnly")]
    [ApiController]
    [Route("api/admin/users")]
    
    [EnableCors("AllowFinanceApp")]
    public class UserAdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserAdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public static class DtoLogger
        {
            public static void LogDto<T>(T dto)
            {
                if (dto == null)
                {
                    Console.WriteLine("DTO is null");
                    return;
                }

                var properties = typeof(T).GetProperties();
                Console.WriteLine($"Logging DTO of type {typeof(T).Name}:");

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(dto);
                    Console.WriteLine($" - {prop.Name}: {value}");
                }
            }
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
            Console.WriteLine("SetRole hit!!!!");
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
                DtoLogger.LogDto(dto);
            }
           

            return usersDto;
        }

        [HttpPost("create")]
        public async Task<bool> CreateAsync([FromBody]ApplicationUserDto dto)
        {
            
            Console.WriteLine(dto);

            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Invalid user data");

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
    }

}
