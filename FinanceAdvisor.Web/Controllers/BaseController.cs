﻿using AutoMapper;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FinanceAdvisor.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly HttpClient _httpClient;
        protected readonly IMapper _mapper;
        protected readonly ITokenRefreshService _tokenService;
        private readonly ILogger<BaseController> _logger;
        private readonly IHttpClientFactory httpClientFactory;
        private IMapper mapper;
        private ITokenRefreshService tokenService;
        protected Guid clientId;
        protected Guid advisorId;

        public BaseController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<BaseController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FinanceAdvisorAPI");
            _mapper = mapper;
            _tokenService = tokenService;
            _logger = logger;

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if(User.Claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User")){
                Console.WriteLine("------------------------------------------------------------------------------------------");
            }

            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            Console.WriteLine(clientIdClaim + "--------------------------------------------------------------------------");
            if (Guid.TryParse(clientIdClaim, out var parsedClientId))
            {
                Console.WriteLine(parsedClientId+"--------------------------------------------------------------------------");
                clientId = parsedClientId;
            }

            var advisorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "advisor_id")?.Value;
            if (Guid.TryParse(advisorIdClaim, out var parsedAdvisorId))
            {
                Console.WriteLine(parsedAdvisorId + "--------------------------------------------------------------------------");
                advisorId = parsedAdvisorId;
            }
        }


        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        protected async Task<IActionResult?> RunChecks(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Unauthorized API response. Signing out user.");

                await HttpContext.SignOutAsync("Cookies");

                var returnUrl = Url.Action("Index", "Home"); // relative path
                var identityLoginUrl = $"https://localhost:7150/Identity/Account/Login?returnUrl={Uri.EscapeDataString(returnUrl)}";
                return Redirect(identityLoginUrl);

            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);

                //var model = new ErrorViewModel { Message = $"API call failed: {response.StatusCode}" };
                //return View("Error", model);
            }

            return null;
        }
    }
}
