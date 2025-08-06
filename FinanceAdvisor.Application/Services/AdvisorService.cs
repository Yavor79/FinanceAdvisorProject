using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Entities;
using FinanceAdvisor.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceAdvisor.Application.Services
{
    public class AdvisorService : IAdvisorService
    {
        private readonly IAdvisorRepository _repo;

        public AdvisorService(IAdvisorRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<AdvisorDto>> GetAllAsync()
        {
            return await _repo.GetAllAttached()
                .Where(a => !a.IsDeleted)
                .Select(a => new AdvisorDto
                {
                    AdvisorId = a.AdvisorId,
                    UserId = a.UserId,
                    Specialization = a.Specialization,
                    CreatedAt = a.CreatedAt,
                    IsDeleted = a.IsDeleted
                }).ToListAsync();
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
                IsDeleted = advisor.IsDeleted
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
                .GetAllAttached()
                .Where(u => specialization == u.Specialization)
                .ToListAsync();

            return advisors.Select(a => new AdvisorDto
            {
                AdvisorId = a.AdvisorId,
                UserId = a.UserId,
                Specialization = a.Specialization,
                CreatedAt = a.CreatedAt,
                IsDeleted = a.IsDeleted
            });
        }

        public async Task CreateAsync(AdvisorDto dto)
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
            var user = await _repo.GetByIdAsync(id);
            if (user == null || !user.IsDeleted) return false;

            user.IsDeleted = false;
            await _repo.UpdateAsync(user);
            return true;
        }
    }
}
