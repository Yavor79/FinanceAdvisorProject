using FinanceAdvisor.Domain.Enums;
//using Microsoft.AspNetCore.Identity;
namespace FinanceAdvisor.Domain.Entities
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public Guid CreditConsultationCycleId { get; set; }
        public CreditConsultationCycle CreditConsultationCycle { get; set; } = null!;
        public DateTime ScheduledDateTime { get; set; }
        public Enums.Type Type { get; set; }
    }
}
