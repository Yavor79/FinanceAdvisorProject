using System;

namespace FinanceAdvisor.Web.Models
{
    public class CreditConsultationCycleViewModel
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid AdvisorId { get; set; }
        public string ClientName { get; set; }
        public string AdvisorName { get; set; }
        public string CreditType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int MeetingCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
