using AutoMapper;
using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Web.Controllers;
using FinanceAdvisor.Web.Helpers;
using FinanceAdvisor.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace FinanceAdvisor.Web.Areas.Admin.Controllers
{
    public class UserManagementController : BaseAdminController
    {
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ITokenRefreshService tokenService,
            ILogger<UserManagementController> logger)
            : base(httpClientFactory, mapper, tokenService, logger)
        {
            _logger = logger;
            
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteWithRefreshAsync($"/api/v1/ApplicationUsers/{id}", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to delete user. Status code: {response.StatusCode}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error", "Unable to delete user.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/ApplicationUsers/restore/{id}", _tokenService);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UpdateApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dto = _mapper.Map<ApplicationUserDto>(model);

                var response = await _httpClient.PutAsJsonWithRefreshAsync($"/api/v1/ApplicationUsers/{id}", dto, _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to update user. Status code: {response.StatusCode}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error", "Unable to update user.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/ApplicationUsers/{id}", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();

                var dto = await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
                var viewModel = _mapper.Map<UpdateApplicationUserViewModel>(dto);

                return View(viewModel);
            }
            catch
            {
                return View("Error", "Unable to load user for editing.");
            }
        }

        public async Task<IActionResult> Deleted()
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync("/api/v1/ApplicationUsers/deleted", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"API Error: {response.StatusCode}");

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ApplicationUserDto>>();
                var viewModels = _mapper.Map<IEnumerable<ApplicationUserViewModel>>(dtos);

                return View(viewModels);
            }
            catch
            {
                return View("Error", "Unable to load deleted users.");
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _httpClient.GetWithRefreshAsync($"/api/v1/ApplicationUsers/{id}", _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();

                var dto = await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
                var viewModel = _mapper.Map<ApplicationUserViewModel>(dto);

                return View(viewModel);
            }
            catch
            {
                return View("Error", "Unable to load user details.");
            }
        }

        public IActionResult Create()
        {
            return View(new CreateApplicationUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateApplicationUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var dto = _mapper.Map<ApplicationUserDto>(model);
                var response = await _identityServerHttpClient.PostAsJsonWithRefreshAsync("/api/admin/users/create", dto, _tokenService);

                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to create user. Status code: {response.StatusCode}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error", "Unable to create user.");
            }
        }

        

        // Users Only
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _identityServerHttpClient.GetWithRefreshAsync("/api/admin/users/usersOnly", _tokenService);
                Console.WriteLine($"Status: {response.StatusCode}");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Content: {content}");
                var checkResult = await RunChecks(response);
                if (checkResult != null)
                    return checkResult;

                if (!response.IsSuccessStatusCode)
                    return View("Error", $"API Error: {response.StatusCode}");

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<ApplicationUserDto>>();
                var viewModels = _mapper.Map<IEnumerable<ApplicationUserViewModel>>(dtos);

                return View(viewModels);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"🔥 Exception: {ex.Message}");
                return View("Error", $"Exception thrown: {ex.Message}");
                return View("Error", "Unable to load users with 'User' role.");
            }
        }

        [HttpPost]
        
        public async Task<IActionResult> UpdateRole(Guid id, Roles role)
        {
            string roleString = role.ToString();
            try
            {
                if (roleString != "User")
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

                    // should create the Domain entity as well!
                    if (roleString == "Advisor")
                    {
                        AdvisorDto advisorDto = new AdvisorDto
                        {
                            UserId = id,
                        };
                        var response2 = await _httpClient.PostAsJsonWithRefreshAsync("api/v1/Advisors/create", advisorDto, _tokenService);
                        Console.WriteLine($"Status: {response2.StatusCode}");
                        var content2 = await response2.Content.ReadAsStringAsync();
                        Console.WriteLine($"Content: {content2}");
                        var checkResult2 = await RunChecks(response2);
                        if (checkResult2 != null)
                            return checkResult2;

                        if (!response2.IsSuccessStatusCode)
                            return View("Error", $"API Error: {response2.StatusCode}");
                    }
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
