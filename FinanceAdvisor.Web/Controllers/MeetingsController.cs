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
        private readonly ILogger<MeetingsController> _logger;

        public MeetingsController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<MeetingsController> logger)
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

                var accessToken = await _tokenService.GetAccessTokenAsync();
                Console.WriteLine($"AccessToken///////////////{accessToken}");
                ViewBag.AccessToken = accessToken;
                
                return View(viewModels);
            }
            catch
            {
                return View("Error", "Unable to load meetings.");
            }
        }

        public async Task<IActionResult> ByCycleId(Guid cycleId)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Meetings/cycle/{cycleId}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<MeetingDto>>();
            var viewModels = _mapper.Map<IEnumerable<MeetingViewModel>>(dtos);
            ViewBag.CycleId = cycleId;
            ViewBag.AccessToken = await _tokenService.GetAccessTokenAsync();
            return View("Index", viewModels);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Meetings/{id}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dto = await response.Content.ReadFromJsonAsync<MeetingDto>();
            var viewModel = _mapper.Map<MeetingViewModel>(dto);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(Guid cycleId)
        {
            var vm = new CreateMeetingViewModel
            {
                CreditConsultationCycleId = cycleId
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMeetingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<CreateMeetingDto>(model);
            var response = await _httpClient.PostAsJsonWithRefreshAsync("/api/v1/Meetings", dto, _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Meetings/{id}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dto = await response.Content.ReadFromJsonAsync<MeetingDto>();
            var model = _mapper.Map<UpdateMeetingViewModel>(dto);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateMeetingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<UpdateMeetingDto>(model);
            var response = await _httpClient.PutAsJsonWithRefreshAsync("/api/v1/Meetings", dto, _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Guid cycleOwnerId)
        {
            var response = await _httpClient.DeleteWithRefreshAsync($"/api/v1/Meetings/{id}/owner/{cycleOwnerId}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Upcoming(DateTime fromDate)
        {
            var url = $"/api/v1/Meetings/upcoming?fromDate={fromDate:s}";
            var response = await _httpClient.GetWithRefreshAsync(url, _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<MeetingDto>>();
            var viewModels = _mapper.Map<IEnumerable<MeetingViewModel>>(dtos);
            return View("Index", viewModels);
        }

        public async Task<IActionResult> CountByCycle(Guid cycleId)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Meetings/count/cycle/{cycleId}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var count = await response.Content.ReadFromJsonAsync<int>();
            ViewBag.Count = count;
            return View("Count"); // You need to create a simple Count.cshtml
        }
    }
}
