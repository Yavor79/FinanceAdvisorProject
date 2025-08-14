using FinanceAdvisor.Application.DTOs;

namespace FinanceAdvisor.Application.Interfaces
{
    public interface IMeetingService
    {
        Task<IEnumerable<MeetingDto>> GetAllAsync();
        Task<IEnumerable<MeetingDto>> GetAllByCycleIdAsync(Guid cycleId);

        Task<MeetingDto?> GetByIdAsync(Guid id);
        Task<MeetingDto?> CreateAsync(CreateMeetingDto dto);
        Task<bool> UpdateAsync(UpdateMeetingDto dto);
        Task<bool> DeleteAsync(Guid id, Guid cycleOwnerId);
        Task<IEnumerable<MeetingDto>> GetUpcomingMeetingsAsync(DateTime fromDate);
        Task<int> CountByCycleIdAsync(Guid cycleId);

    }
}
