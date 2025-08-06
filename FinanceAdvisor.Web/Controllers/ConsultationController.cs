using AutoMapper;
using Azure;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FinanceAdvisor.Web.Controllers
{

    public class ConsultationController : BaseController
    {
        private readonly ILogger<ConsultationController> _logger;

        public ConsultationController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<ConsultationController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }
        public static void Log(CreateConsultationViewModel vm)
        {
            Console.WriteLine("=== CreateConsultationViewModel Log ===");
            Console.WriteLine($"ClientId: {vm.ClientId}");
            Console.WriteLine($"AdvisorId: {vm.AdvisorId}");
            Console.WriteLine($"ScheduledAt: {vm.ScheduledDateTime}");
            Console.WriteLine($"ConsultationType: {vm.ConsultationType}");
            Console.WriteLine("========================================");
        }
        private void LogConsultationDto(string context, object dto)
        {
            Console.WriteLine($"=== {context} DTO Logging Start ===");

            foreach (var prop in dto.GetType().GetProperties())
            {
                var value = prop.GetValue(dto, null);
                Console.WriteLine($"{prop.Name}: {value}");
            }

            Console.WriteLine($"=== {context} DTO Logging End ===");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string filter)
        {
            try
            {
                HttpResponseMessage response;

                // Determine role
                var isAdvisor = User.Claims.Any(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "Advisor");

                var isClient = User.Claims.Any(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "User");

                if (isAdvisor && advisorId != Guid.Empty)
                {
                    var endpoint = string.IsNullOrWhiteSpace(filter)
                        ? $"/api/v1/Consultations/advisor/{advisorId}"
                        : $"/api/v1/Consultations/advisor/{advisorId}/{filter}";

                    response = await _httpClient.GetWithRefreshAsync(endpoint, _tokenService);
                    var checkResult = await RunChecks(response);
                    if (checkResult != null)
                        return checkResult;

                    if (!response.IsSuccessStatusCode)
                        return View("Error", $"API Error: {response.StatusCode}");

                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ConsultationDto>>();
                    var viewModels = _mapper.Map<IEnumerable<ConsultationViewModel>>(dtos);

                    ViewBag.Filter = filter;
                    return View(viewModels);
                }
                if (isClient && clientId != Guid.Empty)
                {
                    var endpoint = string.IsNullOrWhiteSpace(filter)
                        ? $"/api/v1/Consultations/client/{clientId}"
                        : $"/api/v1/Consultations/client/{clientId}/{filter}";

                    response = await _httpClient.GetWithRefreshAsync(endpoint, _tokenService);
                    var checkResult = await RunChecks(response);
                    if (checkResult != null)
                        return checkResult;

                    if (!response.IsSuccessStatusCode)
                        return View("Error", $"API Error: {response.StatusCode}");

                    var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ConsultationDto>>();
                    var viewModels = _mapper.Map<IEnumerable<ConsultationViewModel>>(dtos);

                    ViewBag.Filter = filter;
                    return View(viewModels);
                }

                ViewBag.Filter = filter;
                IEnumerable<ConsultationViewModel> e = new List<ConsultationViewModel>();
                
                return View(e);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load consultations.");
                return View("Error", "Unable to load consultations.");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Consultations/{id}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            var dto = await response.Content.ReadFromJsonAsync<ConsultationDto>();
            var vm = _mapper.Map<ConsultationViewModel>(dto);

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create(ConsultationType filter)
        {
            var model = new CreateConsultationViewModel();

            model.ConsultationType = filter;
            Console.WriteLine("//////////////"+filter.ToString());

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create([Bind("ClientId,AdvisorId,ScheduledDateTime,ConsultationType")] CreateConsultationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Log(model);
            var dto = _mapper.Map<CreateConsultationDto>(model);
            LogConsultationDto("Create", dto);
            var response = await _httpClient.PostAsJsonWithRefreshAsync("/api/v1/Consultations", dto, _tokenService);

            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            var filter = model.ConsultationType switch
            {
                ConsultationType.CreditAdvisory => "Credit",
                ConsultationType.InvestmentAdvisory => "Investment",
                ConsultationType.SecurityAdvisory => "Security",
                _ => null
            };

            return RedirectToAction(nameof(Index), new { filter });
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Consultations/{id}", _tokenService);

            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            var dto = await response.Content.ReadFromJsonAsync<ConsultationDto>();
            var vm = _mapper.Map<UpdateConsultationViewModel>(dto);

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateConsultationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<UpdateConsultationDto>(model);
            var response = await _httpClient.PutAsJsonWithRefreshAsync("/api/v1/Consultations", dto, _tokenService);

            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Guid advisorId)
        {
            var response = await _httpClient.DeleteWithRefreshAsync($"/api/v1/Consultations/{id}/advisor/{advisorId}", _tokenService);

            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CountByClient(Guid clientId)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Consultations/count/client/{clientId}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            var count = await response.Content.ReadFromJsonAsync<int>();

            return View("Count", count);
        }


    }
}

