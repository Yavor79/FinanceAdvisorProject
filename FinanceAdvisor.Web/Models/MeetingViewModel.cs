using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Web.Models
{
    public class MeetingViewModel
    {
        public Guid Id { get; set; }
        public Guid CreditConsultationCycleId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Domain.Enums.Type Type { get; set; }
    }
}
