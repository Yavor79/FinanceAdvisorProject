using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Controllers
{
    public class AdvisorCreditConsultationCycleController : AdvisorBaseController
    {
        private readonly ILogger<AdvisorCreditConsultationCycleController> _logger;

        public AdvisorCreditConsultationCycleController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<AdvisorCreditConsultationCycleController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<CreditConsultationCycleViewModel> viewModels;

            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/CreditConsultationCycle/client/{clientId}", _tokenService);
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



        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCreditConsultationCycleViewModel());
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
            var viewModel = _mapper.Map<UpdateCreditConsultationCycleViewModel>(dto);

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
