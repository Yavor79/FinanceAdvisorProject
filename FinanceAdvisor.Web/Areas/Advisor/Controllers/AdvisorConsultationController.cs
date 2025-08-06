using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Controllers
{

    public class AdvisorConsultationController : AdvisorBaseController
    {
        private readonly ILogger<AdvisorConsultationController> _logger;

        public AdvisorConsultationController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<AdvisorConsultationController> logger)
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


        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Consultations/client/{clientId}", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"API Error: {response.StatusCode}");

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ConsultationDto>>();
                var viewModels = _mapper.Map<IEnumerable<ConsultationViewModel>>(dtos);

                return View(viewModels);
            }
            catch
            {
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
        public IActionResult Create()
        {
            return View(new CreateConsultationViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Create([Bind("ClientId,AdvisorId,ScheduledAt,ConsultationType")] CreateConsultationViewModel model)
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

            return RedirectToAction(nameof(Index));
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

