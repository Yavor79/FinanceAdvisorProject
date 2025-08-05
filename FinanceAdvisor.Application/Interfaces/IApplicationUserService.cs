using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUserDto>> GetAllAsync();
        Task<ApplicationUserDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ApplicationUserDto>> GetAllDeletedAsync();
        Task UpdateAsync(Guid id, ApplicationUserDto dto);
        Task SoftDeleteAsync(Guid id);

        Task<bool> RestoreAsync(Guid id);
        Task<bool> CreateAsync(ApplicationUserDto dto);
    }
}
