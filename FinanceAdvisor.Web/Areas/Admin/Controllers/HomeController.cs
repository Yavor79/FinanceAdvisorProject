using AutoMapper;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseAdminController
    {
        public HomeController(IHttpClientFactory httpClientFactory, IMapper mapper, ITokenRefreshService tokenService, ILogger<BaseController> logger) : base(httpClientFactory, mapper, tokenService, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
