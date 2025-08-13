using AutoMapper;
using Azure.Core;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Common.Logging;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    
    public class ConsultationManagementController : BaseAdminController
    {
        private readonly ILogger<ConsultationManagementController> _logger;

        public ConsultationManagementController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<ConsultationManagementController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }

        

        private Specialization ConvertToSpecialization(ConsultationType type)
        {
            return type switch
            {
                ConsultationType.CreditAdvisory => Specialization.Credit,
                ConsultationType.InvestmentAdvisory => Specialization.Investment,
                ConsultationType.SecurityAdvisory => Specialization.Security,
                _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported consultation type: {type}")
            };
        }

        

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync("/api/v1/Consultations", _tokenService);

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
        public async Task<IActionResult> Create()
        {
            

            var model = new CreateConsultationViewModel();



            var response2 = await _identityServerHttpClient.GetWithRefreshAsync("/api/admin/users/usersOnly", _tokenService);
            var checkResult2 = await RunChecks(response2);
            if (checkResult2 != null) return checkResult2;

            if (!response2.IsSuccessStatusCode)
                return View("Error", $"API Error: {response2.StatusCode}");


            var dto2 = await response2.Content.ReadFromJsonAsync<IEnumerable<ApplicationUserDto>>();
            _logger.LogCollectionProperties(dto2, "[ConsultationManagementController]");
            if (dto2 == null)
            {
                return View(model);
            }
            var vm2 = _mapper.Map<IEnumerable<ChooseUserViewModel>>(dto2);
            _logger.LogCollectionProperties(vm2, "[ConsultationManagementController]");
            model.ChooseUsers = vm2;


            var accessToken = await _tokenService.GetAccessTokenAsync();
            ViewBag.AccessToken = accessToken;
            Console.WriteLine($"////////////////Access token: {accessToken}");
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create([Bind("ClientId,AdvisorId,ScheduledAt,ConsultationType")] CreateConsultationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _logger.LogObjectProperties(model, "[ConsultationManagementController]");
            var dto = _mapper.Map<CreateConsultationDto>(model);
            _logger.LogObjectProperties(dto, "[ConsultationManagementController]");
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

            //var specialization = Specialization.Credit;

            //var response2 = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/specialization/{specialization}", _tokenService);
            //var checkResult2 = await RunChecks(response2);
            //if (checkResult2 != null) return checkResult2;

            //if (!response2.IsSuccessStatusCode)
            //    return View("Error", $"API Error: {response2.StatusCode}");


            //var dto2 = await response2.Content.ReadFromJsonAsync<IEnumerable<AdvisorDto>>();
            //if (dto2 == null)
            //{
            //    return View(vm);
            //}
            //var advisorViewModels = _mapper.Map<IEnumerable<ChooseAdvisorViewModel>>(dto2);
            //vm.ChooseAdvisors = advisorViewModels;

            ViewBag.AccessToken = await _tokenService.GetAccessTokenAsync();
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

        public async Task<IActionResult> Upcoming(Guid advisorId, int daysAhead = 7)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Consultations/upcoming/{advisorId}?daysAhead={daysAhead}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ConsultationDto>>();
            var viewModels = _mapper.Map<IEnumerable<ConsultationViewModel>>(dtos);

            return View("Upcoming", viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCompleted(Guid consultationId, Guid advisorId)
        {
            var response = await _httpClient.PatchWithRefreshAsync($"/api/v1/Consultations/{consultationId}/complete?advisorId={advisorId}", _tokenService);
            var checkResult = await RunChecks(response);
            if (checkResult != null) return checkResult;

            if (!response.IsSuccessStatusCode)
                return View("Error", $"API Error: {response.StatusCode}");

            return RedirectToAction(nameof(Index));
        }
    }
}

