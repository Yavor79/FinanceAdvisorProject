using FinanceAdvisor.Domain.Enums;
//using Microsoft.AspNetCore.Identity;
namespace FinanceAdvisor.Domain.Entities
{
    public class CreditConsultationCycle
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        //public ApplicationUser Client { get; set; } = null!;
        public Guid AdvisorId { get; set; }
        public Advisor Advisor { get; set; } = null!;
        public CreditType CreditType { get; set; }
        public Status Status { get; set; }
        public ICollection<Meeting>? Meetings { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
