using FinanceAdvisor.Domain.Enums;
//using Microsoft.AspNetCore.Identity;
namespace FinanceAdvisor.Domain.Entities
{
    public class Advisor
    {
        public Guid AdvisorId { get; set; }
        public Guid UserId { get; set; }
        // public IdentityUser User { get; set; } = null!;
        public Specialization Specialization { get; set; }
        public ICollection<Consultation>? Consultations { get; set; }
        public ICollection<CreditConsultationCycle>? CreditConsultationCycles { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
