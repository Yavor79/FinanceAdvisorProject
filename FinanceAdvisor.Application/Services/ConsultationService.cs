using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;
using FinanceAdvisor.Domain.Enums;
using FinanceAdvisor.Domain.Entities;
using Microsoft.Extensions.Logging;

using FinanceAdvisor.Common.Logging;


namespace FinanceAdvisor.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _repository;
        private readonly IApplicationUserRepository _userRepository;
        private readonly IAdvisorRepository _advisorRepository;
        private readonly ILogger<ConsultationService> _logger;
        public ConsultationService(IConsultationRepository repository, IApplicationUserRepository applicationUserRepository, IAdvisorRepository advisorRepository, ILogger<ConsultationService> logger)
        {
            _logger = logger;
            _repository = repository;
            _userRepository = applicationUserRepository;
            _advisorRepository = advisorRepository;
        }

        private async Task<ConsultationDto> SetClientAndAdvisorNameInDTO(ConsultationDto cycle)
        {

            _logger.LogObjectProperties(cycle, "[ConsultationService]");
            var user = await _userRepository.GetByIdAsync(cycle.ClientId);
            var userEmail = user?.Email;

            var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId, false);
            var advisorUser = await _userRepository.GetByIdAsync(advisor.UserId);
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


        private async Task<IEnumerable<ConsultationDto>> SetClientAndAdvisorNameInDTO(IEnumerable<ConsultationDto> consultations)
        {


            foreach (ConsultationDto cycle in consultations)
            {
                _logger.LogObjectProperties(cycle, "[ConsultationService]");
                var user = await _userRepository.GetByIdAsync(cycle.ClientId);
                var userEmail = user?.Email;

                var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId, false);
                var advisorUser = await _userRepository.GetByIdAsync(advisor.UserId);
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
            return consultations;
        }

        
        private async Task<IEnumerable<ConsultationDto>> MapWithUserEmailsAsync(IEnumerable<Domain.Entities.Consultation> consultations)
        {
            var clientIds = consultations.Select(c => c.ClientId);
            var advisorUserIds = consultations.Select(c => c.Advisor?.UserId ?? Guid.Empty);
            var allUserIds = clientIds.Concat(advisorUserIds).Distinct();

            var users = await _userRepository.GetByIdsAsync(allUserIds);
            var userDict = users.ToDictionary(u => u.Id, u => u.Email);

            Console.WriteLine("=== Consultation Mapping Started ===");

            var result = consultations.Select(c =>
            {
                var clientEmail = userDict.GetValueOrDefault(c.ClientId, "N/A");
                var advisorId = c.Advisor?.UserId ?? Guid.Empty;
                var advisorEmail = userDict.GetValueOrDefault(advisorId, "N/A");

                Console.WriteLine($"Consultation ID: {c.ConsultationId}");
                Console.WriteLine($"ClientId: {c.ClientId} -> {clientEmail}");
                Console.WriteLine($"AdvisorId: {advisorId} -> {advisorEmail}");
                Console.WriteLine("----------------------------------");

                return new ConsultationDto
                {
                    ConsultationId = c.ConsultationId,
                    ClientId = c.ClientId,
                    AdvisorId = c.AdvisorId,
                    ClientName = clientEmail,
                    AdvisorName = advisorEmail,
                    ScheduledDateTime = c.ScheduledDateTime,
                    Status = c.Status,
                    ConsultationType = c.ConsultationType,
                    CreatedAt = c.CreatedAt
                };
            });

            Console.WriteLine("=== Consultation Mapping Completed ===");

            return result;
        }

        private async Task<ConsultationDto?> MapWithUserEmailsAsync(Domain.Entities.Consultation consultation)
        {
            var result = await MapWithUserEmailsAsync(new[] { consultation });
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllAsync()
        {
            var consultations = await _repository
               .GetAllAttached()
               .Include(c => c.Advisor)
               .AsNoTracking()
               .ToListAsync();

            return await MapWithUserEmailsAsync(consultations);
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId, string? consultationType)
        {
            if (advisorId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            List<Consultation> consultations;

            if (string.IsNullOrWhiteSpace(consultationType))
            {
                consultations = await _repository
                    .GetAllAttached()
                    .Include(c => c.Advisor)
                    .AsNoTracking()
                    .Where(c => c.AdvisorId == advisorId)
                    .ToListAsync();
            }
            else
            {
                if (!Enum.TryParse<ConsultationType>(consultationType, ignoreCase: true, out var parsedType))
                    return Enumerable.Empty<ConsultationDto>(); // invalid consultation type string

                consultations = await _repository
                    .GetAllAttached()
                    .Include(c => c.Advisor)
                    .AsNoTracking()
                    .Where(c => c.AdvisorId == advisorId && c.ConsultationType == parsedType)
                    .ToListAsync();
            }

            return await MapWithUserEmailsAsync(consultations);
        }


        public async Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId, string? consultationType)
        {
            if (clientId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            List<Consultation> consultations;

            if (string.IsNullOrWhiteSpace(consultationType))
            {
                consultations = await _repository
                    .GetAllAttached()
                    .Include(c => c.Advisor)
                    .AsNoTracking()
                    .Where(c => c.ClientId == clientId)
                    .ToListAsync();
            }
            else
            {
                if (!Enum.TryParse<ConsultationType>(consultationType, ignoreCase: true, out var parsedType))
                    return Enumerable.Empty<ConsultationDto>(); // invalid consultation type string

                consultations = await _repository
                    .GetAllAttached()
                    .Include(c => c.Advisor)
                    .AsNoTracking()
                    .Where(c => c.ClientId == clientId && c.ConsultationType == parsedType)
                    .ToListAsync();
            }

            return await MapWithUserEmailsAsync(consultations);
        }


        public async Task<ConsultationDto?> GetByIdAsync(Guid id)
        {
            var c = await _repository
                .GetAllAttached()
                .Include(c => c.Advisor)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConsultationId == id);

            if (c == null) return null;

            return await MapWithUserEmailsAsync(c);
        }

        public async Task<ConsultationDto?> CreateAsync(CreateConsultationDto dto)
        {
            _logger.LogObjectProperties(dto, "[ConsultationService]");
            var consultation = await _repository
                .FirstOrDefaultAsync(c => c.AdvisorId == dto.AdvisorId
                    && c.ClientId == dto.ClientId
                    && c.ScheduledDateTime == dto.ScheduledDateTime);

            if(consultation == null)
            {
                var entity = new Domain.Entities.Consultation
                {
                    ConsultationId = Guid.NewGuid(),
                    ClientId = dto.ClientId,
                    AdvisorId = dto.AdvisorId,
                    ScheduledDateTime = dto.ScheduledDateTime,
                    ConsultationType = dto.ConsultationType,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(entity);

                return await MapWithUserEmailsAsync(entity);
            }

            return null;
        }

        public async Task<bool> UpdateAsync(UpdateConsultationDto dto)
        {
            _logger.LogObjectProperties(dto, "[ConsultationService]");

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            
            if (dto.ScheduledDate.HasValue)
                entity.ScheduledDateTime = dto.ScheduledDate.Value;

            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            if (dto.ConsultationType.HasValue)
                entity.ConsultationType = dto.ConsultationType.Value;

            if (dto.ClientId.HasValue)
                entity.ClientId = dto.ClientId.Value;

            if (dto.AdvisorId.HasValue)
                entity.AdvisorId = dto.AdvisorId.Value;

            return await _repository.UpdateAsync(entity);
        }


        public async Task<bool> DeleteSelfAsync(Guid consultationId, Guid advisorId)
        {
            var entity = await _repository.GetByIdAsync(consultationId);
            if (entity == null || entity.AdvisorId != advisorId)
                return false;

            return await _repository.DeleteAsync(entity);
        }

        public async Task<bool> MarkAsCompletedAsync(Guid consultationId, Guid advisorId)
        {
            var entity = await _repository.GetByIdAsync(consultationId);
            if (entity == null || entity.AdvisorId != advisorId)
                return false;

            entity.Status = Domain.Enums.Status.Completed;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<int> CountByClientIdAsync(Guid clientId)
        {
            return await _repository
                .GetAllAttached()
                .CountAsync(c => c.ClientId == clientId);
        }

        public async Task<IEnumerable<ConsultationDto>> GetUpcomingForAdvisorAsync(Guid advisorId, int daysAhead = 7)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysAhead);

            var consultations = await _repository
                .GetAllAttached()
                .Include(c => c.Advisor)
                .AsNoTracking()
                .Where(c => c.AdvisorId == advisorId && c.ScheduledDateTime >= DateTime.UtcNow && c.ScheduledDateTime <= targetDate)
                .OrderBy(c => c.ScheduledDateTime)
                .ToListAsync();

            return await MapWithUserEmailsAsync(consultations);
        }

    }
}
