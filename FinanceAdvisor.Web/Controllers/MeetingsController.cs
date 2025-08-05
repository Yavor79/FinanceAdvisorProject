using FinanceAdvisor.Web.Models;
using FinanceAdvisor.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using AutoMapper;
using FinanceAdvisor.Web.Helpers;

namespace FinanceAdvisor.Web.Controllers
{
    public class MeetingsController : BaseController
    {
        private readonly ILogger<CreditConsultationsCycleController> _logger;

        public MeetingsController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<CreditConsultationsCycleController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync("/api/v1/Meetings", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"API Error: {response.StatusCode}");

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<MeetingDto>>();
                var viewModels = _mapper.Map<IEnumerable<MeetingViewModel>>(dtos);

                return View(viewModels);
            }
            catch
            {
                return View("Error", "Unable to load meetings.");
            }
        }
    }
}
