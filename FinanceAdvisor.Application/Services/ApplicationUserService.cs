using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;

namespace FinanceAdvisor.Application.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IApplicationUserRepository _repo;

        public ApplicationUserService(IApplicationUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllAsync()
        {
            return await _repo.GetAllAttached()
                .Where(u => !u.IsDeleted)
                .Select(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    IsDeleted = u.IsDeleted
                }).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllDeletedAsync()
        {
            return await _repo.GetAllAttached()
                .Where(u => u.IsDeleted)
                .Select(u => new ApplicationUserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    
                    CreatedAt = u.CreatedAt,
                    IsDeleted = u.IsDeleted
                }).ToListAsync();
        }

        public async Task<ApplicationUserDto?> GetByIdAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user == null ? null : new ApplicationUserDto
            {
                Id = user.Id,
                Email = user.Email,
                
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted
            };
        }

        public async Task UpdateAsync(Guid id, ApplicationUserDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return;

            user.Email = dto.Email;
            
            await _repo.UpdateAsync(user);
            
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return;

            user.IsDeleted = true;
            await _repo.UpdateAsync(user);
            
        }
    }
}
