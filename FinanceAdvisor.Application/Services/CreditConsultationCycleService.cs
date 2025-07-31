using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;


namespace FinanceAdvisor.Application.Services
{
    public class CreditConsultationCycleService : ICreditConsultationCycleService
    {
        private readonly ICreditConsultationCycleRepository _repository;

        public CreditConsultationCycleService(ICreditConsultationCycleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CreditConsultationCycleDto>> GetAllAsync()
        {
            IEnumerable<CreditConsultationCycleDto> cycles = await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Select(c => new CreditConsultationCycleDto
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    CreditType = c.CreditType,
                    Status = c.Status,
                    MeetingCount = c.Meetings != null ? c.Meetings.Count : 0,
                    CreatedAt = c.CreatedAt
                })
                .ToArrayAsync();

            return cycles;
        }

        public async Task<IEnumerable<CreditConsultationCycleDto>> GetAllByManagerIdAsync(Guid? managerId)
        {
            if (managerId == null || managerId == Guid.Empty)
                return Enumerable.Empty<CreditConsultationCycleDto>();

            IEnumerable<CreditConsultationCycleDto> cycles = await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.AdvisorId == managerId)
                .Select(c => new CreditConsultationCycleDto
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    CreditType = c.CreditType,
                    Status = c.Status,
                    MeetingCount = c.Meetings != null ? c.Meetings.Count : 0,
                    CreatedAt = c.CreatedAt
                })
                .ToArrayAsync();

            return cycles;
        }
        public async Task<IEnumerable<CreditConsultationCycleDto>> GetAllByClientIdAsync(Guid? Id)
        {
            if (Id == null || Id == Guid.Empty)
            { return Enumerable.Empty<CreditConsultationCycleDto>(); }

            IEnumerable<CreditConsultationCycleDto> cycles = await this._repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.ClientId.ToString().ToLower() == Id.ToString().ToLower())
                .Select(c => new CreditConsultationCycleDto()
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    CreditType = c.CreditType,
                    Status = c.Status,
                    MeetingCount = c.Meetings != null ? c.Meetings.Count : 0,
                    CreatedAt = c.CreatedAt

                }).ToArrayAsync();
            return cycles;
        }
    }
}
