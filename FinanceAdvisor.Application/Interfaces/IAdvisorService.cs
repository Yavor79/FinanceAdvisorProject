using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IAdvisorService
    {
        Task<IEnumerable<AdvisorDto>> GetAllAsync();
        Task<AdvisorDto?> GetByIdAsync(Guid id);
        Task<AdvisorDto?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<AdvisorDto>> GetBySpecializationAsync(Specialization specialization);
        Task<bool> CreateAsync(AdvisorDto dto);
        Task UpdateAsync(Guid advisorId, AdvisorDto dto);
        Task SoftDeleteAsync(Guid advisorId);
        Task<bool> RestoreAsync(Guid id);
    }
}
