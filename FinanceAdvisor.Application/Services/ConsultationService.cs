using FinanceAdvisor.Application.DTOs;
using FinanceAdvisor.Application.Interfaces;
using FinanceAdvisor.Application.IRepos;
using Microsoft.EntityFrameworkCore;


namespace FinanceAdvisor.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _repository;
        private readonly IApplicationUserRepository _userRepository;
        private readonly IAdvisorRepository _advisorRepository;
        public ConsultationService(IConsultationRepository repository, IApplicationUserRepository applicationUserRepository, IAdvisorRepository advisorRepository)
        {
            _repository = repository;
            _userRepository = applicationUserRepository;
            _advisorRepository = advisorRepository;
        }

        private async Task<ConsultationDto> SetClientAndAdvisorNameInDTO(ConsultationDto cycle)
        {

            PrintConsultationDto(cycle);
            var user = await _userRepository.GetByIdAsync(cycle.ClientId);
            var userEmail = user?.Email;

            var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId);
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
                PrintConsultationDto(cycle);
                var user = await _userRepository.GetByIdAsync(cycle.ClientId);
                var userEmail = user?.Email;

                var advisor = await _advisorRepository.GetByIdAsync(cycle.AdvisorId);
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

        public static void PrintConsultationDto(ConsultationDto dto, string context = "ConsultationDto")
        {
            Console.WriteLine($"===== {context} =====");
            Console.WriteLine($"ConsultationId     : {dto.ConsultationId}");
            Console.WriteLine($"ClientId           : {dto.ClientId}");
            Console.WriteLine($"ClientName         : {dto.ClientName}");
            Console.WriteLine($"AdvisorId          : {dto.AdvisorId}");
            Console.WriteLine($"AdvisorName        : {dto.AdvisorName}");
            Console.WriteLine($"ScheduledDateTime  : {dto.ScheduledDateTime}");
            Console.WriteLine($"Status             : {dto.Status}");
            Console.WriteLine($"ConsultationType   : {dto.ConsultationType}");
            Console.WriteLine($"CreatedAt          : {dto.CreatedAt}");
            Console.WriteLine("=============================\n");
        }

        private void LogConsultationDto(string context, object dto)
        {
            Console.WriteLine($"=== {context} DTO Logging Start ===");

            foreach (var prop in dto.GetType().GetProperties())
            {
                var value = prop.GetValue(dto, null);
                Console.WriteLine($"{prop.Name}: {value}");
            }

            Console.WriteLine($"=== {context} DTO Logging End ===");
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

        public async Task<IEnumerable<ConsultationDto>> GetAllByAdvisorIdAsync(Guid advisorId)
        {
            if (advisorId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            var consultations = await _repository
                .GetAllAttached()
                .Include(c => c.Advisor)
                .AsNoTracking()
                .Where(c => c.AdvisorId == advisorId)
                .ToListAsync();

            return await MapWithUserEmailsAsync(consultations);
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllByClientIdAsync(Guid clientId)
        {
            if (clientId == Guid.Empty)
                return Enumerable.Empty<ConsultationDto>();

            var consultations = await _repository
                .GetAllAttached()
                .Include(c => c.Advisor)
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .ToListAsync();

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
            LogConsultationDto("Create", dto);

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

        public async Task<bool> UpdateAsync(UpdateConsultationDto dto)
        {
            LogConsultationDto("Update", dto);

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            entity.ScheduledDateTime = dto.ScheduledDate;
            entity.Status = dto.Status;
            entity.ConsultationType = dto.ConsultationType;

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
