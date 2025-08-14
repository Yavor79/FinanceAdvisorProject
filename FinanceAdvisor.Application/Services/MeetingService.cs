using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Common.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceAdvisor.Application.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly IMeetingRepository _repository;
        private readonly ILogger<MeetingService> _logger;

        public MeetingService(IMeetingRepository repository, ILogger<MeetingService> logger)
        {
            _repository = repository;
            _logger = logger;
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

        
        public async Task<MeetingDto?> GetByIdAsync(Guid id)
        {
            var m = await _repository.GetByIdAsync(id);
            if (m == null) return null;

            return new MeetingDto
            {
                Id = m.Id,
                CreditConsultationCycleId = m.CreditConsultationCycleId,
                ScheduledDateTime = m.ScheduledDateTime,
                Type = m.Type
            };
        }

        public async Task<MeetingDto?> CreateAsync(CreateMeetingDto dto)
        {
            var meetingExists = await _repository
                .FirstOrDefaultAsync(m => m.CreditConsultationCycleId == dto.CreditConsultationCycleId
                && m.ScheduledDateTime == dto.ScheduledDateTime);

            if(meetingExists == null)
            {
                var meeting = new Domain.Entities.Meeting
                {
                    Id = Guid.NewGuid(),
                    CreditConsultationCycleId = dto.CreditConsultationCycleId,
                    ScheduledDateTime = dto.ScheduledDateTime,
                    Type = dto.Type
                };

                _logger.LogObjectProperties(meeting, "[MeetingService]");

                await _repository.AddAsync(meeting);

                return new MeetingDto
                {
                    Id = meeting.Id,
                    CreditConsultationCycleId = meeting.CreditConsultationCycleId,
                    ScheduledDateTime = meeting.ScheduledDateTime,
                    Type = meeting.Type
                };
            }

            return null;
        }

        public async Task<bool> UpdateAsync(UpdateMeetingDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            if(dto.ScheduledDateTime.HasValue) { entity.ScheduledDateTime = dto.ScheduledDateTime.Value; }
            
            if(dto.Type.HasValue) { entity.Type = dto.Type.Value; }

            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Guid id, Guid cycleOwnerId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            // Optionally check ownership if needed
            // You can extend Meeting entity with AdvisorId if needed

            return await _repository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<MeetingDto>> GetUpcomingMeetingsAsync(DateTime fromDate)
        {
            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(m => m.ScheduledDateTime > fromDate)
                .Select(m => new MeetingDto
                {
                    Id = m.Id,
                    CreditConsultationCycleId = m.CreditConsultationCycleId,
                    ScheduledDateTime = m.ScheduledDateTime,
                    Type = m.Type
                })
                .ToListAsync();
        }

        public async Task<int> CountByCycleIdAsync(Guid cycleId)
        {
            return await _repository
                .GetAllAttached()
                .CountAsync(m => m.CreditConsultationCycleId == cycleId);
        }
        

    }
}
