//using Microsoft.AspNetCore.Identity;
using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Domain.Entities
{
    public class Consultation
    {
        public Guid ConsultationId { get; set; }
        public Guid ClientId { get; set; }
        //public ApplicationUser Client { get; set; } = null!;
        public Guid AdvisorId { get; set; }
        public Advisor Advisor { get; set; } = null!;
        public DateTime ScheduledDateTime { get; set; }
        public Status Status { get; set; }
        public ConsultationType ConsultationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
