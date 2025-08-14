using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

using FinanceAdvisor.Common.Logging;
using Microsoft.Extensions.Logging;

namespace FinanceAdvisor.Application.Services
{
    public class AdvisorService : IAdvisorService
    {
        private readonly IAdvisorRepository _repo;
        private readonly IApplicationUserRepository _userRepo;
        private readonly ILogger<AdvisorService> _logger;

        public AdvisorService(
            IAdvisorRepository repo,
            IApplicationUserRepository userRepo,
            ILogger<AdvisorService> logger)
        {
            _repo = repo;
            _userRepo = userRepo;
            _logger = logger;
        }


        private async Task<string> GetNameByIdAsync(Guid id)
        {
           
            var user = await _userRepo.GetByIdAsync(id);
            return user.Email;
        }

        public async Task<IEnumerable<AdvisorDto>> GetAllAsync()
        {
            var advisors = await _repo.GetAllAttached(true)
            .ToListAsync();


            var advisorDtos = new List<AdvisorDto>();

            foreach (var a in advisors)
            {
                var advisorName = await GetNameByIdAsync(a.UserId);

                advisorDtos.Add(new AdvisorDto
                {
                    AdvisorId = a.AdvisorId,
                    AdvisorName = advisorName,
                    UserId = a.UserId,
                    Specialization = a.Specialization,
                    CreatedAt = a.CreatedAt,
                    IsDeleted = a.IsDeleted
                });
            }

            return advisorDtos;
        }

        public async Task<AdvisorDto?> GetByIdAsync(Guid id)
        {
            var advisor = await _repo.GetByIdAsync(id);
            return advisor == null ? null : new AdvisorDto
            {
                AdvisorId = advisor.AdvisorId,
                UserId = advisor.UserId,
                Specialization = advisor.Specialization,
                CreatedAt = advisor.CreatedAt,
                IsDeleted = advisor.IsDeleted,
                Email = await GetNameByIdAsync(advisor.AdvisorId)
            };
        }

        public async Task<AdvisorDto?> GetByUserIdAsync(Guid userId)
        {
            var advisor = await _repo.FirstOrDefaultAsync(u => userId == u.UserId);
            return advisor == null ? null : new AdvisorDto
            {
                AdvisorId = advisor.AdvisorId,
                UserId = advisor.UserId,
                Specialization = advisor.Specialization,
                CreatedAt = advisor.CreatedAt,
                IsDeleted = advisor.IsDeleted
            };
        }

        public async Task<IEnumerable<AdvisorDto>> GetBySpecializationAsync(Specialization specialization)
        {
            var advisors = await _repo
                .GetAllAttached(false)
                .Where(u => specialization == u.Specialization)
                .ToListAsync();

            var advisorDtos = new List<AdvisorDto>();

            foreach (var a in advisors)
            {
                var advisorName = await GetNameByIdAsync(a.UserId);

                var dto = new AdvisorDto
                {
                    AdvisorId = a.AdvisorId,
                    AdvisorName = advisorName,
                    UserId = a.UserId,
                    Specialization = a.Specialization,
                    CreatedAt = a.CreatedAt,
                    IsDeleted = a.IsDeleted
                };

                // Log the DTO
                _logger.LogObjectProperties(dto, "[AdvisorService]");

                advisorDtos.Add(dto);
            }

            return advisorDtos;
        }

        public async Task<bool> CreateAsync(AdvisorDto dto)
        {
            var advisorExists = await _repo
                .FirstOrDefaultAsync(a => a.UserId == dto.UserId);
            if(advisorExists == null)
            {
                var advisor = new Advisor
                {
                    AdvisorId = Guid.NewGuid(),
                    UserId = dto.UserId,
                    Specialization = dto.Specialization != 0 ? dto.Specialization : Specialization.Credit,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _repo.AddAsync(advisor);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task UpdateAsync(Guid advisorId, AdvisorDto dto)
        {
            var advisor = await _repo.GetByIdAsync(advisorId);
            if (advisor == null) return;

            advisor.Specialization = dto.Specialization;
            await _repo.UpdateAsync(advisor);
            
        }

        public async Task SoftDeleteAsync(Guid advisorId)
        {
            var advisor = await _repo.GetByIdAsync(advisorId);
            if (advisor == null) return;

            advisor.IsDeleted = true;
            await _repo.UpdateAsync(advisor);
            
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id, true);
            Console.WriteLine($"user{user}");
            if (user == null || !user.IsDeleted) return false;

            user.IsDeleted = false;
            await _repo.UpdateAsync(user);
            return true;
        }
    }
}
