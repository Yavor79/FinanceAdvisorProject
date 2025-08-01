using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;

namespace FinanceAdvisor.Application.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _repository;

        public MeetingService(IMeetingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MeetingDto>> GetAllAsync()
        {
            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Select(m => new MeetingDto
                {
                    Id = m.Id,
                    CreditConsultationCycleId = m.CreditConsultationCycleId,
                    ScheduledDateTime = m.ScheduledDateTime,
                    Type = m.Type
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<MeetingDto>> GetAllByCycleIdAsync(Guid cycleId)
        {
            if (cycleId == Guid.Empty)
                return Enumerable.Empty<MeetingDto>();

            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(m => m.CreditConsultationCycleId == cycleId)
                .Select(m => new MeetingDto
                {
                    Id = m.Id,
                    CreditConsultationCycleId = m.CreditConsultationCycleId,
                    ScheduledDateTime = m.ScheduledDateTime,
                    Type = m.Type
                })
                .ToArrayAsync();
        }
    }
}
