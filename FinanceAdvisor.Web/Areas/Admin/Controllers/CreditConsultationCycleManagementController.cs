using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    public class CreditConsultationCycleManagementController : BaseAdminController
    {
        private readonly ILogger<CreditConsultationCycleManagementController> _logger;

        public CreditConsultationCycleManagementController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<CreditConsultationCycleManagementController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }

        private void LogSingleObjectProperties<T>(string context, T? collection)
        {
            if (collection == null)
            {
                Console.WriteLine($"=== {context} collection is null ===");
                return;
            }

            foreach (var prop in collection.GetType().GetProperties())
            {
                var value = prop.GetValue(collection, null);
                Console.WriteLine($"{prop.Name}: {value}");
            }
               
            Console.WriteLine($"=== {context} collection logging end ===");
        }

        private void LogObjectProperties<T>(string context, IEnumerable<T>? collection)
        {
            if (collection == null)
            {
                Console.WriteLine($"=== {context} collection is null ===");
                return;
            }

            Console.WriteLine($"=== {context} collection logging start ({collection.Count()} items) ===");

            int index = 1;
            foreach (var item in collection)
            {
                Console.WriteLine($"--- Item {index} ---");
                foreach (var prop in item.GetType().GetProperties())
                {
                    var value = prop.GetValue(item, null);
                    Console.WriteLine($"{prop.Name}: {value}");
                }
                index++;
            }

            Console.WriteLine($"=== {context} collection logging end ===");
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CreditConsultationCycleViewModel> viewModels;

            try
            {
                var response = await _httpClient.GetWithRefreshAsync("/api/v1/CreditConsultationCycle", _tokenService);
                Console.WriteLine("1 ***************************");
                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<CreditConsultationCycleDto>>();

                // Map to ViewModels
                viewModels = dtos != null
                    ? _mapper.Map<IEnumerable<CreditConsultationCycleViewModel>>(dtos)
                    : new List<CreditConsultationCycleViewModel>();
            }
            catch (Exception ex)
            {
                // Handle unreachable API, timeout, etc.
                Console.WriteLine("Unable to load consultation cycles.");
                Console.WriteLine(ex);
                return View("Error", "Unable to load consultation cycles.");
            }

            return View(viewModels);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/CreditConsultationCycle/{id}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            var dto = await response.Content.ReadFromJsonAsync<CreditConsultationCycleDto>();
            var viewModel = _mapper.Map<CreditConsultationCycleViewModel>(dto);

            return View(viewModel);
        }

        public async Task<IActionResult> Pending()
        {
            var response = await _httpClient.GetWithRefreshAsync("/api/v1/CreditConsultationCycle/pending", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<CreditConsultationCycleDto>>();
            var viewModels = _mapper.Map<IEnumerable<CreditConsultationCycleViewModel>>(dtos);
            return View("Index", viewModels);
        }

        public async Task<IActionResult> CountByAdvisor(Guid advisorId)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/CreditConsultationCycle/count/advisor/{advisorId}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            var count = await response.Content.ReadFromJsonAsync<int>();
            ViewBag.Count = count;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateCreditConsultationCycleViewModel();

            var specialization = Specialization.Credit;
           
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/specialization/{specialization}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");


            var dto = await response.Content.ReadFromJsonAsync<IEnumerable<AdvisorDto>>();
            if (dto == null)
            {
                return View(model);
            }
            var vm = _mapper.Map<IEnumerable<ChooseAdvisorViewModel>>(dto);
            model.ChooseAdvisors = vm;

            var response2 = await _identityServerHttpClient.GetWithRefreshAsync("/api/admin/users/usersOnly", _tokenService);
            var checkResult2 = await RunChecks(response2);
            if (checkResult2 != null) return checkResult2;

            if (!response2.IsSuccessStatusCode)
                return View("Error", $"API Error: {response2.StatusCode}");


            var dto2 = await response2.Content.ReadFromJsonAsync<IEnumerable<ApplicationUserDto>>();
            LogObjectProperties("ApplicationUserDto", dto2);
            if (dto2 == null)
            {
                return View(model);
            }
            var vm2 = _mapper.Map<IEnumerable<ChooseUserViewModel>>(dto2);
            LogObjectProperties("ChooseUserViewModel", vm2);
            model.ChooseUsers = vm2;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCreditConsultationCycleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<CreateCreditConsultationCycleDto>(model);
            var response = await _httpClient.PostAsJsonWithRefreshAsync("/api/v1/CreditConsultationCycle", dto, _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/CreditConsultationCycle/{id}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            var dto = await response.Content.ReadFromJsonAsync<CreditConsultationCycleDto>();
            LogSingleObjectProperties("CreditConsultationCycleDto", dto);
            var viewModel = _mapper.Map<UpdateCreditConsultationCycleViewModel>(dto);
            LogSingleObjectProperties("UpdateCreditConsultationCycleViewModel", viewModel);
            var specialization = Specialization.Credit;

            var response2 = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/specialization/{specialization}", _tokenService);
            var checkResult2 = await RunChecks(response2);
            if (checkResult2 != null) return checkResult2;

            if (!response2.IsSuccessStatusCode)
                return View("Error", $"API Error: {response2.StatusCode}");


            var dto2 = await response2.Content.ReadFromJsonAsync<IEnumerable<AdvisorDto>>();
            if (dto2 == null)
            {
                return View(viewModel);
            }
            var vm = _mapper.Map<IEnumerable<ChooseAdvisorViewModel>>(dto2);
            viewModel.ChooseAdvisors = vm;
            LogSingleObjectProperties("UpdateCreditConsultationCycleViewModel", viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCreditConsultationCycleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<UpdateCreditConsultationCycleDto>(model);
            var response = await _httpClient.PutAsJsonWithRefreshAsync("/api/v1/CreditConsultationCycle", dto, _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Guid advisorId)
        {
            var response = await _httpClient.DeleteWithRefreshAsync($"/api/v1/CreditConsultationCycle/{id}/advisor/{advisorId}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null)
                return checkResult;

            return RedirectToAction(nameof(Index));
        }

    }
}
