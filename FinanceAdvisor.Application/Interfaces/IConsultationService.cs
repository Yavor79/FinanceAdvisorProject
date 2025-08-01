using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<IEnumerable<ConsultationDto>> GetAllAsync();
        Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId);
        Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId);
    }
}
