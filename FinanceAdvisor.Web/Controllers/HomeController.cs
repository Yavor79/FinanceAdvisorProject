using AutoMapper;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace FinanceAdvisor.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ILogger<CreditConsultationsCycleController> _logger;

        public HomeController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<CreditConsultationsCycleController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            //Debug();
            //var claims = User.Claims
            //.Select(c => $"{c.Type}: {c.Value}")
            //.ToList();

            //return Content(string.Join("\n", claims));
            //var roles = User.Claims
            //.Where(c => c.Type == "role")
            //.Select(c => $"{c.Type} = {c.Value}");

            //return Content("Roles:\n" + string.Join("\n", roles));
            if (User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin"))
            {
                Console.WriteLine("***/////////////////////////Admin");
            }
            else if(User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Advisor"))
            {
                Console.WriteLine("---------/////////////////////////ADvisor");
            }
            else if (User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User"))
            {
                Console.WriteLine("//***************/////////////////////////User");
            }
            else { Console.WriteLine("/////////////////////////Nooooooooo"); }


            return View();
        }

        public IActionResult Debug()
        {
            var claims = User.Claims
            .Select(c => $"{c.Type}: {c.Value}")
            .ToList();

            return Content(string.Join("\n", claims));
        }

        public IActionResult Privacy()
        {
            var claims = User.Claims
            .Select(c => $"{c.Type}: {c.Value}")
            .ToList();

            return Content(string.Join("\n", claims));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
