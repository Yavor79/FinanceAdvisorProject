using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Controllers
{
    public class AdminAccountController : Controller
    {
        

        [HttpGet]
        public IActionResult AdminLogout()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "https://localhost:7053/" },
                "Cookies", "oidc");
        }
    }
}
