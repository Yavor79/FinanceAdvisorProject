using AutoMapper;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace FinanceAdvisor.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<HomeController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Contacts()
        {
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
           
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is InvalidOperationException ioe && ioe.Message.Contains("The view"))
            {
                // Optionally, you could log the exception or inspect it further
                _logger.LogWarning("Missing view detected. Redirecting to FunctionalityToBeImplemented.");
                return this.View("FunctionalityToBeImplemented");
            }

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