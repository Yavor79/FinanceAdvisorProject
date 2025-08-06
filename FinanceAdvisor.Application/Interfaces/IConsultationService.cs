using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<IEnumerable<ConsultationDto>> GetAllAsync();
        Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId, string? consultationType);
        Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId, string? consultationType);

        Task<ConsultationDto?> CreateAsync(CreateConsultationDto dto);
        Task<bool> UpdateAsync(UpdateConsultationDto dto);
        Task<bool> DeleteSelfAsync(Guid consultationId, Guid advisorId);
        Task<ConsultationDto?> GetByIdAsync(Guid id);
        Task<bool> MarkAsCompletedAsync(Guid consultationId, Guid advisorId);
        Task<int> CountByClientIdAsync(Guid clientId);
        Task<IEnumerable<ConsultationDto>> GetUpcomingForAdvisorAsync(Guid advisorId, int daysAhead = 7);

    }
}
