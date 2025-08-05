using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<IEnumerable<ConsultationDto>> GetAllAsync();
        Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId);
        Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId);

        Task<ConsultationDto?> CreateAsync(CreateConsultationDto dto);
        Task<bool> UpdateAsync(UpdateConsultationDto dto);
        Task<bool> DeleteSelfAsync(Guid consultationId, Guid advisorId);
        Task<ConsultationDto?> GetByIdAsync(Guid id);
        Task<bool> MarkAsCompletedAsync(Guid consultationId, Guid advisorId);
        Task<int> CountByClientIdAsync(Guid clientId);
        Task<IEnumerable<ConsultationDto>> GetUpcomingForAdvisorAsync(Guid advisorId, int daysAhead = 7);

    }
}
