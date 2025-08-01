using FinanceAdvisor.Web.Models;
using FinanceAdvisor.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using AutoMapper;

namespace FinanceAdvisor.Web.Controllers
{
    public class MeetingsController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public MeetingsController(IHttpClientFactory httpClientFactory, IMapper mapper)
            : base(httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinanceAdvisorAPI");
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/v1/Meetings");
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
