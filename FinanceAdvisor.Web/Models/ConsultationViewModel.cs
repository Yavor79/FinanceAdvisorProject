using FinanceAdvisor.Domain.Enums;

namespace FinanceAdvisor.Web.Models
{
    public class ConsultationViewModel
    {
        public Guid ConsultationId { get; set; }
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public string ClientName { get; set; }
        public string AdvisorName { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public Status Status { get; set; }
        public ConsultationType ConsultationType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
