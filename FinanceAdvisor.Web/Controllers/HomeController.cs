using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinanceAdvisor.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        //IManagerService managerService;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _logger = logger;
        }

       
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
