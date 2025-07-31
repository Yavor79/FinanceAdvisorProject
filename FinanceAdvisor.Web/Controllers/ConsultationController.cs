using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Controllers
{
    public class ConsultationController : BaseController
    {
        public ConsultationController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
