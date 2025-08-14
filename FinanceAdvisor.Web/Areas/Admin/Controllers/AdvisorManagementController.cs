using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    public class AdvisorManagementController : BaseAdminController
    {
        private readonly ILogger<AdvisorManagementController> _logger;

        public AdvisorManagementController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<AdvisorManagementController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync("/api/v1/Advisors", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"API Error: {response.StatusCode}");

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AdvisorDto>>();
                var viewModels = _mapper.Map<IEnumerable<AdvisorViewModel>>(dtos);

                return View(viewModels);
            }
            catch
            {
                return View("Error", "Unable to load advisors.");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/{id}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dto = await response.Content.ReadFromJsonAsync<AdvisorDto>();
            var viewModel = _mapper.Map<AdvisorViewModel>(dto);
            return View(viewModel);
        }

        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/user/{userId}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dto = await response.Content.ReadFromJsonAsync<AdvisorDto>();
            var viewModel = _mapper.Map<AdvisorViewModel>(dto);
            return View("Details", viewModel);
        }

        public async Task<IActionResult> BySpecialization(Specialization specialization)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/specialization/{specialization}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<AdvisorDto>>();
            var viewModels = _mapper.Map<IEnumerable<AdvisorViewModel>>(dtos);
            return View("Index", viewModels);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<AdvisorDto>(model);
            var response = await _httpClient.PostAsJsonWithRefreshAsync("/api/v1/Advisors", dto, _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/{id}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            var dto = await response.Content.ReadFromJsonAsync<AdvisorDto>();
            var model = _mapper.Map<UpdateAdvisorViewModel>(dto);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateAdvisorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = _mapper.Map<AdvisorDto>(model);
            var response = await _httpClient.PutAsJsonWithRefreshAsync($"/api/v1/Advisors/{model.AdvisorId}", dto, _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _httpClient.DeleteWithRefreshAsync($"/api/v1/Advisors/{id}", _tokenService);
            var check = await RunChecks(response);
            if (check != null) return check;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        
        public async Task<IActionResult> Restore(Guid id)
        {
            //TODO: check if current advisor was implemented with User role and delete it before restore
            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/Advisors/restore/{id}", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to restore user. Status code: {response.StatusCode}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error", "Unable to restore user.");
            }
        }

        public async Task<IActionResult> UpdateRole(Guid id, Guid advisorId, Roles role)
        {
            string roleString = role.ToString();
            
            try
            {
                if (roleString != "Advisor")
                {
                    var response = await _identityServerHttpClient.PostAsJsonWithRefreshAsync(
                        $"/api/admin/users/{id}/role",
                        roleString, _tokenService);
                    Console.WriteLine($"Status: {response.StatusCode}");
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                    var check = await RunChecks(response);
                    if (check != null)
                        return check;

                    if (!response.IsSuccessStatusCode)
                        return View("Error", "Failed to update user role.");

                    var resp = await _httpClient.DeleteWithRefreshAsync($"/api/v1/Advisors/{advisorId}", _tokenService);
                    var _check = await RunChecks(resp);
                    if (_check != null) return _check;

                }
                

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Exception: {ex.Message}");
                return View("Error", $"Exception thrown: {ex.Message}");
            }

        }
    }
}
