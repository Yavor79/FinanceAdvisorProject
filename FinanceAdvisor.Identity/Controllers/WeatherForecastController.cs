using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.AuthServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // This endpoint requires a valid access token
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            var userName = User.Identity?.Name ?? "unknown";
            return Ok(new
            {
                Message = "This is a secure endpoint!",
                User = userName
            });
        }

        // This endpoint is public
        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult GetPublicData()
        {
            return Ok(new
            {
                Message = "This is a public endpoint!"
            });
        }
    }
}
