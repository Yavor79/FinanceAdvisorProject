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
    
    public class AdvisorHomeController : AdvisorBaseController
    {
        private readonly ILogger<AdvisorHomeController> _logger;

        public AdvisorHomeController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<AdvisorHomeController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {

            if (User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Admin"))
            {
                Console.WriteLine("***/////////////////////////Admin");
            }
            else if (User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Advisor"))
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
        public IActionResult Error(int? statusCode)
        {
            switch (statusCode)
            {
                case 401:
                    return this.View("Error401");
                case 403:
                    return this.View("UnauthorizedError");
                case 404:
                    return this.View("NotFoundError");
                case 500:
                    return this.View("Error500");
                default:
                    return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}