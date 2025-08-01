using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;


namespace FinanceAdvisor.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _repository;

        public ConsultationService(IConsultationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllAsync()
        {
            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Select(c => new ConsultationDto
                {
                    ConsultationId = c.ConsultationId,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    ScheduledDateTime = c.ScheduledDateTime,
                    Status = c.Status,
                    ConsultationType = c.ConsultationType,
                    CreatedAt = c.CreatedAt
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId)
        {
            if (advisorId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.AdvisorId == advisorId)
                .Select(c => new ConsultationDto
                {
                    ConsultationId = c.ConsultationId,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    ScheduledDateTime = c.ScheduledDateTime,
                    Status = c.Status,
                    ConsultationType = c.ConsultationType,
                    CreatedAt = c.CreatedAt
                })
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId)
        {
            if (clientId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => new ConsultationDto
                {
                    ConsultationId = c.ConsultationId,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    ScheduledDateTime = c.ScheduledDateTime,
                    Status = c.Status,
                    ConsultationType = c.ConsultationType,
                    CreatedAt = c.CreatedAt
                })
                .ToArrayAsync();
        }
    }
}
