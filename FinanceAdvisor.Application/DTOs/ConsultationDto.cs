using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Application.DTOs
{
    public class ConsultationDto
    {
        public Guid ConsultationId { get; set; }
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Status Status { get; set; }
        public ConsultationType ConsultationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
