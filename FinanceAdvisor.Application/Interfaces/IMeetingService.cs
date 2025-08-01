using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IMeetingService
    {
        Task<IEnumerable<MeetingDto>> GetAllAsync();
        Task<IEnumerable<MeetingDto>> GetAllByCycleIdAsync(Guid cycleId);
    }
}
