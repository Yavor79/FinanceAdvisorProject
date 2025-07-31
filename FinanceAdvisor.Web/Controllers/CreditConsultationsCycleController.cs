using FinanceAdvisor.Web.Models;
using FinanceAdvisor.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace FinanceAdvisor.Web.Controllers
{
    public class CreditConsultationsCycleController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public CreditConsultationsCycleController(IHttpClientFactory httpClientFactory, IMapper mapper) : base(httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FinanceAdvisorAPI");
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CreditConsultationCycleViewModel> viewModels;

            try
            {
                var response = await _httpClient.GetAsync("/api/v1/CreditConsultationCycle");
                if (!response.IsSuccessStatusCode)
                {

                    // Log error, optionally display a warning or fallback
                    Console.WriteLine($"API call failed: {response.StatusCode}");
                    return View("Error", $"API call failed: {response.StatusCode}");
                    
                }

                var dtos = await response.Content.ReadFromJsonAsync<IEnumerable<CreditConsultationCycleDto>>();

                // Map to ViewModels
                viewModels = dtos != null
                    ? _mapper.Map<IEnumerable<CreditConsultationCycleViewModel>>(dtos)
                    : new List<CreditConsultationCycleViewModel>();
            }
            catch
            {
                // Handle unreachable API, timeout, etc.
                Console.WriteLine("Unable to load consultation cycles.");
                return View("Error", "Unable to load consultation cycles.");
            }

            return View(viewModels);
        }

        private static IEnumerable<CreditConsultationCycleViewModel> MapToViewModels(IEnumerable<CreditConsultationCycleDto> dtos)
        {
            return dtos.Select(dto => new CreditConsultationCycleViewModel
            {
                Id = dto.Id,
                ClientId = dto.ClientId,
                AdvisorId = dto.AdvisorId,
                CreditType = dto.CreditType.ToString(),
                Status = dto.Status.ToString(),
                MeetingCount = dto.MeetingCount,
                CreatedAt = dto.CreatedAt
            });
        }
    }
}
