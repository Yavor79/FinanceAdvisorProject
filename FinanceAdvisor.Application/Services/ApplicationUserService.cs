using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Enums;
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
            //var users = await _repo.GetFilteredAsync(u => !u.IsDeleted);
            // In Application Layer:
            var allUsers = await _repo.GetAllAsync();
            var filtered = allUsers.Where(u => !u.IsDeleted).ToList();


            return filtered.Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                IsDeleted = u.IsDeleted
            });
        }


        public async Task<IEnumerable<ApplicationUserDto>> GetAllDeletedAsync()
        {
            var allUsers = await _repo.GetAllAsync();
            var filtered = allUsers.Where(u => u.IsDeleted).ToList();


            return filtered.Select(u => new ApplicationUserDto
            {
                Id = u.Id,
                Email = u.Email,

                CreatedAt = u.CreatedAt,
                IsDeleted = u.IsDeleted
            });

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

        public async Task<bool> RestoreAsync(Guid id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null || !user.IsDeleted) return false;

            user.IsDeleted = false;
            await _repo.UpdateAsync(user);
            return true;
        }

        public async Task<bool> CreateAsync(ApplicationUserDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Invalid user data");
            var UserExists = await _repo.GetByIdAsync(dto.Id);
            if(UserExists == null)
            {
                var newUser = new Domain.Entities.ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };


                return await _repo.AddAsync(newUser);
            }

            return false;
        }

        
        //public async Task<IEnumerable<ApplicationUserDto>> SearchByEmailAsync(string keyword)
        //{
        //    return await _repo.GetAllAttached()
        //        .Where(u => !u.IsDeleted && u.Email.Contains(keyword))
        //        .Select(u => new ApplicationUserDto
        //        {
        //            Id = u.Id,
        //            Email = u.Email,
        //            CreatedAt = u.CreatedAt,
        //            IsDeleted = u.IsDeleted
        //        }).ToListAsync();
        //}

    }
}
