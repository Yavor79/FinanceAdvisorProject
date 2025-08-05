using Microsoft.AspNetCore.Mvc;
using FinanceAdvisor.Web.Controllers;
using AutoMapper;
using FinanceAdvisor.Web.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    [Authorize(Policy="AdminOnly")]
    [Area("Admin")]
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(IHttpClientFactory httpClientFactory, IMapper mapper, ITokenRefreshService tokenService, ILogger<BaseController> logger) : base(httpClientFactory, mapper, tokenService, logger)
        {
        }

        
    }
}
