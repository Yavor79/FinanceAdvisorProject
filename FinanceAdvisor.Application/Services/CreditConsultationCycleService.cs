using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using FinanceAdvisor.Domain.Enums;
using Microsoft.EntityFrameworkCore;


namespace FinanceAdvisor.Application.Services
{
    public class CreditConsultationCycleService : ICreditConsultationCycleService
    {
        private readonly ICreditConsultationCycleRepository _repository;
        private readonly IAdvisorRepository _advisorRepository;
        private readonly IApplicationUserRepository _applicationUserRepository;

        public CreditConsultationCycleService(ICreditConsultationCycleRepository repository, IApplicationUserRepository applicationUserRepository, IAdvisorRepository advisorRepository)
        {
            _repository = repository;
            _advisorRepository = advisorRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        private async Task<CreditConsultationCycleDto> SetClientAndAdvisorNameInDTO(CreditConsultationCycleDto cycle)
        {
            var user = await _applicationUserRepository.GetByIdAsync(cycle.ClientId);
            var userEmail = user?.Email;

            var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId, true);
            var advisorUser = await _applicationUserRepository.GetByIdAsync(advisor.UserId);
            var advisorUserEmail = advisorUser?.Email;

            if (userEmail != null)
            {
                cycle.ClientName = userEmail;
            }
            if (advisorUserEmail != null)
            {
                cycle.AdvisorName = advisorUserEmail;
            }

            return cycle;
        }


        private async Task<IEnumerable<CreditConsultationCycleDto>> SetClientAndAdvisorNameInDTO(IEnumerable<CreditConsultationCycleDto> cycles)
        {
            foreach (CreditConsultationCycleDto cycle in cycles)
            {
                var user = await _applicationUserRepository.GetByIdAsync(cycle.ClientId);
                var userEmail = user?.Email;

                // Advisor has Query Filter => if he's deleted the repo won't find him
                var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId, true);
                var advisorUser = await _applicationUserRepository.GetByIdAsync(advisor.UserId);
                var advisorUserEmail = advisorUser?.Email;

                if (userEmail != null)
                {
                    cycle.ClientName = userEmail;
                }
                if (advisorUserEmail != null)
                {
                    cycle.AdvisorName = advisorUserEmail;
                }

            }
            return cycles;
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

            cycles = await SetClientAndAdvisorNameInDTO(cycles);

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

            cycles = await SetClientAndAdvisorNameInDTO(cycles);
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

            cycles = await SetClientAndAdvisorNameInDTO(cycles);

            return cycles;
        }

        public async Task<CreditConsultationCycleDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            CreditConsultationCycleDto cycle = new CreditConsultationCycleDto
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                AdvisorId = entity.AdvisorId,
                CreditType = entity.CreditType,
                Status = entity.Status,
                MeetingCount = entity.Meetings?.Count ?? 0,
                CreatedAt = entity.CreatedAt
            };
            cycle = await SetClientAndAdvisorNameInDTO(cycle);
            return cycle;
        }

        public async Task<bool> CreateAsync(CreateCreditConsultationCycleDto dto)
        {
            

            var newCycle = new Domain.Entities.CreditConsultationCycle
            {
                Id = Guid.NewGuid(),
                ClientId = dto.ClientId,
                AdvisorId = dto.AdvisorId,
                CreditType = dto.CreditType,
                Status = Status.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newCycle);

            //var cycle = await SetClientAndAdvisorNameInDTO(new CreditConsultationCycleDto
            //{
            //    Id = newCycle.Id,
            //    ClientId = newCycle.ClientId,
            //    AdvisorId = newCycle.AdvisorId,
            //    CreditType = newCycle.CreditType,
            //    Status = newCycle.Status,
            //    MeetingCount = 0,
            //    CreatedAt = newCycle.CreatedAt
            //});
            return true;

        }

        public async Task<bool> UpdateAsync(UpdateCreditConsultationCycleDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            entity.CreditType = dto.CreditType;
            entity.Status = dto.Status;

            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteSelfAsync(Guid id, Guid advisorId)
        {
            Console.WriteLine("got to service ddelete111111111111111111111111111111");
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null || entity.AdvisorId != advisorId)
                return false;
            Console.WriteLine("got to service ddelete222222222222222222222");
            return await _repository.DeleteAsync(entity);
        }

        public async Task<int> CountByAdvisorAsync(Guid advisorId)
        {
            return await _repository
                .GetAllAttached()
                .CountAsync(c => c.AdvisorId == advisorId);
        }

        public async Task<IEnumerable<CreditConsultationCycleDto>> GetPendingCyclesAsync()
        {
            return await _repository
                .GetAllAttached()
                .AsNoTracking()
                .Where(c => c.Status == Status.Pending)
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
                .ToListAsync();
        }

    }
}
